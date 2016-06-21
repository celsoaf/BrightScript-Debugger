'///////////////////////////////////////////////////////////////
'//
'//  $copyright: Copyright (C) 2013 BSKYB
'//
'///////////////////////////////////////////////////////////////
function VideoPlayerController() as Object
  return ObjectUtils().extend("VideoPlayerController", CoreController(), {

'----------------------------------------
' Dependencies
'----------------------------------------  

    programmeDetailsModel:ProgrammeDetailsModel()
    videoOptionsModel:VideoOptionsModel()
    videoDataModel: VideoDataModel()
    copy: AppLabels()
    popupMgr: PopupManager()
    popFactory: PopupFactory()
    stringUtils: StringUtils()
    logger: Logger()
    userService: UserService()
    xhr: Xhr()
    playbackService: PlaybackService()
    deviceStatusModel: DeviceStatusModel()
    deviceService: DeviceService()
    videoQualityMonitorService: ConvivaMonitorService()
    executionService: ExecutionService()
    boxSetDetailsService: BoxSetDetailsService()
    typeUtils: TypeUtils()
'----------------------------------------
' Properties
'----------------------------------------  

    section: "Video"
    title: "Video"
    heartbeatService: Invalid
    userServiceInstance: Invalid
    concurrencyControlModel: Invalid
    hasError: false
    timeout: Invalid
    _episodesWatched: 0
    id: "VideoPlayerController"
    playNextData: Invalid
'----------------------------------------
' Public API 
'----------------------------------------

  onBackPressed: function() as Void
    if(m.view.state.isTrailer = true)
      m.onExitPlaySuccess({
        choice: 1
      })
    else
      'Check if the movie has started playing, if not user is trying to exit too quickly!
      if m.view.isLoading
        if m.videoDataService <> Invalid
          m.videoDataService.cancelRequests()
        end if

        m.onExitPlaySuccess({
          choice: 1
        })
      else if m.view.videoPanels <> Invalid and m.view.videoPanels.isEndOfPlayWidgetActive()
        m.view.videoPanels.performEndOfPlayWidgetCancel()
      else        
        m.view.stopAll()
        m.popupMgr.show(m.popFactory.getTwoChoicePopupComponent( "<h1>"  + m.copy.getText("confirmStopMovie") + "</h1>" , "", m.copy.getText("yes"), m.copy.getText("no")),{
          destination: m
          success: "onExitPlaySuccess"
          failed: "onExitPlayFailed"
        })
      end if
    end if
  end function

  bookmark: function(position as Integer, startupTime="" as String) as Void
    m.heartbeatService.bookmark(position, startupTime)
  end function

  heartbeat: function() as Void
    if m.concurrencyControlModel <> Invalid
      m.heartbeatService.concurrencyHeartbeat(m.concurrencyControlModel.getHref(), m.createConcurrencyBody(position))
    else
      m.bookmark(m.view.getPlaybackPosition())
    end if
  end function

  setTimeout: function(resultObj) as Void
    if( m.stringUtils.toString(resultObj.code) = "1")
      m.timeout = resultObj.timeout
    else
      m.showConcurrencyMessage()
    end if
  end function

  loadNextEpisode: function(eventObj) as Void
    m.view.showLoading()
    m.playNextData = eventObj.target     
    m.position = 0

    if m.typeUtils.isInteger(m.playNextData.seasonnumber)
      m.playNextData.seasonIdx = m.programmeDetailsModel.getSeasonIndexFromSeasonNumber( m.playNextData.seasonnumber)
    else
      m.playNextData.seasonIdx = -1
    end if  

    if m.playNextData.seasonIdx = -1
      m.handleVideoError(ErrorPrefixes().CONSUME, Errors().MALFORMED_DATA)
    else if m.programmeDetailsModel.currentProgrammeDetails.currentSeasonNumber <> m.playNextData.seasonIdx
      m._loadNextSeasonData(m.playNextData.seasonIdx)
    else
      m.playNextData.episodeIdx = m.programmeDetailsModel.getEpisodeIndexFromSeasonIndexAndEpisodeNumber(m.playNextData.seasonIdx, m.playNextData.episodenumber)    
      if m.playNextData.episodeIdx = -1
        m.handleVideoError(ErrorPrefixes().CONSUME, Errors().MALFORMED_DATA)
      else
        m.currentEpisode = m.playNextData.episodeIdx
        m.programmeDetailsModel.currentProgrammeDetails.currentSeasonNumber = m.playNextData.seasonIdx
        m.currentVideoOptions = m.playNextData.videoOptions.endpoint
        m.currentSelectedAssetId = m.playNextData.assetId

        m.userService.reAuthenticate({
            destination: m
            failed: "_loadNextEpisodeFailed"
            success: "_loadNextEpisodeReautSuccess"
        })
      end if
    end if
  end function

  _loadNextSeasonData: function(seasonIdx) as Void  
      m.boxSetDetailsService.addEventListener(m, m.boxSetDetailsService.EPISODE_DETAILS_UPDATED, "_onEpisodeDetailsUpdated")
      m.boxSetDetailsService.getAllEpisodeDetails(m.programmeDetailsModel.currentProgrammeDetails.seasons[seasonIdx].href)
  end function

  _onEpisodeDetailsUpdated: function(eventObj) as Void
    m.boxSetDetailsService.removeEventListener(m, m.boxSetDetailsService.EPISODE_DETAILS_UPDATED, "_onEpisodeDetailsUpdated")
    m.programmeDetailsModel.currentProgrammeDetails.currentSeasonNumber = m.playNextData.seasonIdx
    m.loadNextEpisode( {target: m.playNextData } )
  end function

  _loadNextEpisodeReautSuccess: function(resultObj as Object) as Void     
    m.playbackService.addEventListener(m, m.playbackService.PLAYBACK_SUCCESS, "playnextPlayBackSuccessHandler")
    m.playbackService.addEventListener(m, m.playbackService.PLAYBACK_ERROR_SECONDARY_USER, "playbackFailedHandler")
    m.playbackService.addEventListener(m, m.playbackService.PLAYBACK_ERROR_DEVICE_LIMIT, "playbackFailedHandler")
    m.playbackService.addEventListener(m, m.playbackService.PLAYBACK_ERROR_MOVIE_EXPIRED, "playbackFailedHandler")
    m.playbackService.addEventListener(m, m.playbackService.PLAYBACK_ERROR_VIDEO_OPTIONS, "playbackFailedHandler")
    m.playbackService.addEventListener(m, m.playbackService.PLAYBACK_ERROR_DEVICE_STATUS_FAIL, "playbackServiceDeviceStatusFailedHandler")

    m.playbackService.startPlayback(m.programmeDetailsModel.currentProgrammeDetails.entitlement, 0, m.playNextData.episodenumber, m.playNextData.id, true)
  end function  

  playnextPlayBackSuccessHandler: function(eventObj as Object) as Void
    if eventObj.target <> Invalid
      state = StateVO()
      state.isTrailer = false
      state.position = 0
      state.episode = m.playNextData.episodeIdx
      state.episodeId = m.playNextData.id
      state.videoData = m.playNextData.videoOptions.endpoint
      m.navigationManager.navigateTo(Routes().VIDEO_PLAYER, state)
    else
      m.handleVideoError(ErrorPrefixes().CONSUME, Errors().MALFORMED_DATA)
    end if
    m.playNextData = Invalid
  end function

  _loadNextEpisodeFailed: function(resultObj) as Void
      m.executionService.execute(m.analyticsService, "addEvent", [m.analyticsService.EVENT_TYPES.signOut])
      m.userService.signOut({
        destination: m
        success: "_menuRefreshSuccess"
        failed: "_menuRefreshFailed"
      })
      m.userService.hasPendingReauthentication = false    
      m.playNextData = Invalid
  end function

  _menuRefreshSuccess: function(resultObj as Object)
    m.navigationManager.clearAll()
    m.navigationManager.navigateTo(Routes().SHOWCASE, st)
  end function

  _menuRefreshFailed: function(resultObj as Object)
    m.executionService.execute(m.analyticsService, "setError", ["MyAccountController.menuRefreshFailed"])
  end function

  playbackFailedHandler: function(eventObj as Object) as Void
    errorMsg = m.playbackService.getErrorMessage(eventObj.type)
    callback = {
        destination: m
        success: "_onCloseDefaultPopup"
        failed: "_onCloseDefaultPopup"
    }
    m.popupMgr.show(m.popFactory.getConfirmPopupComponent(errorMsg.title, errorMsg.text, m.copy.getText("ok")), callback)          
  end function

  _onCloseDefaultPopup: function(eventObj as Object) as Object
    m.onExitPlaySuccess({
      choice: 1
    })
  end function

  getTimeout: function()
    return m.timeout
  end function

  loadVideoData: function(endpoint as String, isTrailer as boolean) as Void
    if isTrailer = true
      m._loadTrailer(endpoint)
    else
      m._loadVideo(endpoint)
    end if 
  end function

  onGetTrailerDataFailed: function(resultObj as Object) as Void 
    m.handleVideoError(m.view.errorPrefix,resultObj.target.errorCode)
  end function 

  onGetTrailerDataSuccess: function(resultObj as Object) as Void 
    pd = m.programmeDetailsModel.currentProgrammeDetails
    vod = m.videoOptionsModel.currentVideoOptions
    vd = VideoDataValueObject()
    vd.deliveryUrl = resultObj.target.deliveryURL
    m.videoDataModel.currentVideoData = vd

    if m.view.populate <> Invalid 
      m.view.populate(pd, vod, vd)
      m.startPlay()
    end if 
  end function 

  handleVideoError: function(errorPrefix,errorCode) as Void
    if m.hasError = false
      m.hasError = true
      m.view.stopAll()

      onPopupComplete = {
        destination: m
        success: "onErrorPopupComplete"
        failed: "onErrorPopupComplete"
      }
      ErrorService().showPopup(errorPrefix,errorCode,onPopupComplete)
    end if
  end function

  onErrorPopupComplete: function(valResult as Object) as Void
    m.goBackToPDP()
  end function

  goBackToPDP: function(availableToWatch = true) as Object
    m.programmeDetailsModel.resetEpisodesWatched()
    if availableToWatch
      m.releaseConcurrentSteam(m.view.getPlaybackPosition())
    end if

    currentProgramme = m.programmeDetailsModel.currentProgrammeDetails
    if m.view.state.isTrailer = true AND m.programmeDetailsModel.currentProgrammeDetails.isboxset()
      state = m.view.state
      route = m.getRouteBasedOnAssetType(currentProgramme.assetType)
      stepsToRemove = m.navigationManager.howManyStepsSince(route) + 1
      m.navigationManager.popItemsFromHistory(stepsToRemove)
      m.navigationManager.navigateTo(route, state, stepsToRemove)
    else
      route = m.getRouteBasedOnAssetType(currentProgramme.assetType)
      m.navigationManager.goBackTo(route)
    end if 
  end function

  getRouteBasedOnAssetType: function (assetType as Object) as Object
    if assetType = AssetTypes().EPISODE
      route = Routes().BOXSET_DETAILS
    else if assetType = AssetTypes().BOXSET
      route = Routes().BOXSET_DETAILS
    else if assetType = AssetTypes().SEASON
      route =  Routes().SEASON_DETAILS
    else if assetType = AssetTypes().MOVIE
      route = Routes().PROGRAMME_DETAILS
    else
      route = Routes().SHOWCASE
    end if
    return route
  end function

  releaseConcurrentSteam: function(position as Integer) as Void
    if m.heartbeatService <> Invalid and m.concurrencyControlModel <> Invalid
      m.heartbeatService.releaseConcurrentSteam(m.concurrencyControlModel.getHref(), m.createConcurrencyBody(position))
    end if
  end function

'----------------------------------------
' Handlers
'----------------------------------------

  onExitPlaySuccess: function(eventObj as Object) as Void
    if eventObj.choice = 1
      m.executionService.execute(m.analyticsService, "addEvent", [m.analyticsService.EVENT_TYPES.videoStop])
      m.goBackToPDP(true)
      m.heartbeatService.removeEventListener(m, m.heartbeatService.HEARTBEAT_EVENT, "checkConcurrencyToPlay")
      m.heartbeatService.removeEventListener(m, m.heartbeatService.HEARTBEAT_EVENT_FAILED, "concurrencyError")
      if AppUtils().isOptimized()
        eval("m.videoQualityMonitorService.stopMonitoring()")
      else
        m.videoQualityMonitorService.stopMonitoring()
      end if
    else
      m.view.resumePlay()
    end if
  end function

  onExitPlayFailed: function(eventObj as Object) as Void
    m.view.resumePlay()
  end function

  onGetVideoDataSuccess: function(resultObj as Object) as Void
    m.videoDataModel.currentVideoData = resultObj.target
    m.concurrencyControlModel = resultObj.target.concurrencycontrol
    m.heartbeatService.setConcurrencyControlModel(m.concurrencyControlModel)

    m.heartbeatService.addEventListener(m, m.heartbeatService.HEARTBEAT_EVENT, "checkConcurrencyToPlay")
    m.heartbeatService.addEventListener(m, m.heartbeatService.HEARTBEAT_EVENT_FAILED, "concurrencyError")
    m.heartbeatService.addEventListener(m, m.heartbeatService.CONCURRENCY_TIME_EVENT, "setTimeout")

    callback = {
        destination : m
        success: "_registerPlaybackStartSuccess"
        failed: "_registerPlaybackStartFailed"
    }
    m.heartbeatService.registerPlaybackStart(callback)
    currentProgrammeDetails = m.programmeDetailsModel.currentProgrammeDetails
    if currentProgrammeDetails.assetType = AssetTypes().MOVIE
      pd = currentProgrammeDetails
      vod = pd.entitlement
      vd = m.videoDataModel.currentVideoData
    else
      pd  =  m.programmeDetailsModel.getAssetInformation(m.view.state.episode)
      vod  =  m.programmeDetailsModel.getVideoOptions(m.view.state.episode)
      vd = m.videoDataModel.currentVideoData
    end if

    if pd = Invalid
      m.handleVideoError(ErrorPrefixes().CONSUME, Errors().MALFORMED_DATA)
    else
      m.view.populate(pd, vod, vd)
    end if
  end function

  _onGetVideoDataFailed: function(resultObj as Object) as Void
    m.getFullVideoFailed(resultObj.target)
  end function

  resumeDataSuccessHandler: function(resultObj as Object) as Void
    m.videoDataService.removeEventListener(m, m.videoDataService.RESUME_DATA_SUCCESS, "resumeDataSuccessHandler")
    m.videoDataService.removeEventListener(m, m.videoDataService.RESUME_DATA_FAILED, "resumeDataFailedHandler")
    
    pd  =  m.programmeDetailsModel.getAssetInformation(m.view.state.episode)
    vod  =  m.programmeDetailsModel.getVideoOptions(m.view.state.episode)
    vd = m.videoDataModel.currentVideoData    
    m.view.populate(pd, vod, vd)
  end function

  resumeDataFailedHandler: function(resultObj as Object) as Void
    m.videoDataService.removeEventListener(m, m.videoDataService.RESUME_DATA_SUCCESS, "resumeDataSuccessHandler")
    m.videoDataService.removeEventListener(m, m.videoDataService.RESUME_DATA_FAILED, "resumeDataFailedHandler")
 
    m.getFullVideoFailed(resultObj.target)
  end function


  _registerPlaybackStartSuccess: function(resultObj = {} as Object) as boolean
    if not m.stringUtils.isInvalidOrWhiteSpace(m.concurrencyControlModel.getHref())
      m.heartbeatService.concurrencyHeartbeat(m.concurrencyControlModel.getHref(), Invalid)
    else 
      m.startPlay()
      m.view.stopHeartbeat()
    end if
  end function
  
  _registerPlaybackStartFailed: function(resultObj = {} as Object) as boolean
    m.startPlay()
    m.view.stopHeartbeat()
  end function

  checkConcurrencyToPlay: function(eventObj as Object) as Void
    if eventObj.code = 1
      m.startPlay()
    else
      m.showConcurrencyMessage()
    end if
  end function

  showConcurrencyMessage: function() as Void
      concurrencyMessage = m.copy.getText("concurrency_rental")
      if m.programmeDetailsModel.currentProgrammeDetails.entitlement.entitlementType = "EST"
        concurrencyMessage = m.copy.getText("concurrency_est")
      end if
      m.popupMgr.show(m.popFactory.getConfirmPopupComponent("<p>" + concurrencyMessage + "</p>", "", m.copy.getText("ok")), {
        destination: m
        success: "onExitMessage"
        failed: "onExitMessage"
      })
  end function

  concurrencyError: function(resultObj as Object) as Void
    m.logger.warn("VideoPayerController.checkConcurrencyToPlay")
    if resultObj.forceStop = true
      m.heartbeatService.removeEventListener(m, m.heartbeatService.HEARTBEAT_EVENT_FAILED, "concurrencyError")
      m.handleVideoError(ErrorPrefixes().CONSUME, resultObj.errorCode)
    end if
  end function

  onExitMessage: function(resultObj as Object) as Void
    m.goBackToPDP(false)
  end function

  createConcurrencyBody: function(time as Integer) as Object
    concurrencyBody = m.concurrencyControlModel.getConcurrencyBody()
    if concurrencyBody <> Invalid AND concurrencyBody.keyValues <> Invalid
      keyValues = concurrencyBody.keyValues
      for each keyValue in keyValues
        keyValues[keyValue] = str(time)
      end for
    end if
    return concurrencyBody
  end function

  getFullVideoFailed: function(resultObj as Object) as Void
    m.logger.warn("VideoPayerController.getFullVideoFailed")
    m.handleVideoError(ErrorPrefixes().CONSUME, resultObj.errorCode)
  end function

  setUserServiceInstance: function (userServiceInstance as Object)
    m.userServiceInstance = userServiceInstance
  end function

'----------------------------------------
' Private API
'----------------------------------------

  _loadTrailer: function(endpoint as String) as Void 
      m.videoDataService.addEventListener(m, m.videoDataService.TRAILER_DATA_SUCCESS, "onGetTrailerDataSuccess")
      m.videoDataService.addEventListener(m, m.videoDataService.TRAILER_DATA_FAILED, "onGetTrailerDataFailed")
      m.videoDataService.getTrailerData(endpoint)
  end function 

  _loadVideo: function(endpoint as String) as Void
      m.videoDataService.addEventListener(m, m.videoDataService.VIDEO_DATA_SUCCESS, "onGetVideoDataSuccess")
      m.videoDataService.addEventListener(m, m.videoDataService.VIDEO_DATA_FAILED, "_onGetVideoDataFailed")
      m.videoDataService.getVideoData(endpoint)
  end function 

  getUserServiceInstance: function () as Object
    if m.userServiceInstance = Invalid
      m.setUserServiceInstance(UserService())
    end if
    return m.userServiceInstance
  end function

  handleHeartbeat: function(eventObj as Object) as Void
    if m.concurrencyControlModel <> Invalid AND not m.stringUtils.isInvalidOrWhiteSpace(m.concurrencyControlModel.getHref())
      m.heartbeatService.concurrencyHeartbeat(m.concurrencyControlModel.getHref(), m.createConcurrencyBody(m.view.getPlaybackPosition()))
    else
      m.view.stopHeartbeat()
    end if
  end function

  handleVideoComplete: function(eventObj as Object) as Void
    m.goBackToPDP()
  end function

  handleVideoLoaded: function(eventObj as Object) as Void
    m.pubSub.publish(PubSubEvents().HIDE_BACKGROUND)
  end function

  handleBookmark: function(eventObj as Object) as Void
    result = eventObj.target
    pd = m.programmeDetailsModel.currentProgrammeDetails
    m.bookmark(result.position, result.buffertime)
  end function
  
  postInit: function() as Void
    if FeatureCapabilities().videoQualityMonitoring
      if AppUtils().isOptimized()
        eval("m.setupVideoQualityMonitoringSettings()")
      else
        m.setupVideoQualityMonitoringSettings()
      end if
      model = VideoQualityMonitorModel()
      m.pubSub.publish(PubSubEvents().VIDEO_QUALITY_VIDEO_PLAYER_INIT, model)      
    end if
  end function

  startPlay: function() as Void
    m.view.playVideo()
  end function

  setupVideoQualityMonitoringSettings: function() as Void
      videoQMonitorModel = VideoQualityMonitorModel()
      videoQMonitorModel.clear()

      streamUrl = VideoDataModel().currentVideoData.deliveryUrl
      videoQMonitorModel.setStreamUrl(streamUrl)
      videoQMonitorModel.setViewerId(m.deviceService.getUniqueID())

      assetInfo = {}
      assetInfo.isMovieBoxSet = false
      assetInfo.isTvBoxSet = false

      pgrDetailsModel = ProgrammeDetailsModel().currentProgrammeDetails                  
      ctype = ArrayUtils().join(pgrDetailsModel.genre, ",")

      assetInfo.contentType = ctype

      if m.view.state.isTrailer
        assetInfo.entitlementId = ""
      else
        assetInfo.entitlementId = pgrDetailsModel.entitlement.id
      end if

      assetName = ""
      if pgrDetailsModel.isTvBoxSet()
        assetInfo.isTvBoxSet = true 
        assetInfo.boxSetId = pgrDetailsModel.id
        seasonData = Invalid
        curSeasonIdx = ProgrammeDetailsModel().getCurrentSeasonIndex()
        if pgrDetailsModel.seasons <> Invalid AND pgrDetailsModel.seasons.count() >= curSeasonIdx
          seasonData = pgrDetailsModel.seasons[curSeasonIdx]
          if seasonData <> Invalid
            
            seasonNr = seasonData.seasonnumber
            if seasonNr <> Invalid
              assetInfo.season = m.stringUtils.toString(seasonNr)
            else
              assetInfo.season = ""
            end if

            episodeData = seasonData.episodes[m.view.state.episode]
            if episodeData <> Invalid
              assetName = episodeData.title
              assetInfo.episode = m.stringUtils.toString(episodeData.episodenumber)
              assetInfo.assetId = m.stringUtils.toString(episodeData.id)
            end if

          end if
        end if
      else if pgrDetailsModel.isMovieBoxSet()
        assetInfo.isMovieBoxSet = true
        assetInfo.boxSetId = pgrDetailsModel.id

        if m.view.state.isTrailer
          movieData = pgrDetailsModel.movies[m.view.state.assetNumber]
        else
          movieData = pgrDetailsModel.movies[m.view.state.episode]
        end if

        if movieData <> Invalid
          assetName = movieData.title
          assetInfo.assetId = m.stringUtils.toString(movieData.id)
        end if
      else
        assetInfo.assetId = pgrDetailsModel.id
        assetName = pgrDetailsModel.title
      end if

      videoQMonitorModel.setAssetName(assetName)
      assetInfo.contentName = pgrDetailsModel.title

      videoQMonitorModel.setAssetInfo(assetInfo)

    end function

'----------------------------------------
' Override
'----------------------------------------

    handleDispose: function() as Void
      m.xhr.bypassMaintenanceMode = false
      m.videoDataService.removeEventListener(m, m.videoDataService.VIDEO_DATA_SUCCESS, "onGetVideoDataSuccess")
      m.videoDataService.removeEventListener(m, m.videoDataService.VIDEO_DATA_FAILED, "onGetVideoDataFailed")
      m.heartbeatService.removeEventListener(m, m.heartbeatService.CONCURRENCY_TIME_EVENT, "setTimeout")
      m.playbackService.removeEventListener(m, m.playbackService.PLAYBACK_SUCCESS, "playnextPlayBackSuccessHandler")
      m.playbackService.removeEventListener(m, m.playbackService.PLAYBACK_ERROR_SECONDARY_USER, "playbackFailedHandler")
      m.playbackService.removeEventListener(m, m.playbackService.PLAYBACK_ERROR_DEVICE_LIMIT, "playbackFailedHandler")
      m.playbackService.removeEventListener(m, m.playbackService.PLAYBACK_ERROR_MOVIE_EXPIRED, "playbackFailedHandler")
      m.playbackService.removeEventListener(m, m.playbackService.PLAYBACK_ERROR_VIDEO_OPTIONS, "playbackFailedHandler")
      m.playbackService.removeEventListener(m, m.playbackService.PLAYBACK_ERROR_DEVICE_STATUS_FAIL, "playbackServiceDeviceStatusFailedHandler")      
      m.view.removeEventListener(m, m.view.HEARTBEAT, "handleHeartbeat")
      m.view.removeEventListener(m, m.view.VIDEO_COMPLETE, "handleVideoComplete")
      m.view.removeEventListener(m, m.view.VIDEO_LOADED, "handleVideoLoaded")
      m.view.removeEventListener(m, m.view.BOOKMARK, "handleBookmark")
      m.heartbeatService.deregisterPlaybackStart(m.id)
      if AppUtils().isOptimized()
        eval("m.videoQualityMonitorService.stopMonitoring()")
      else
        m.videoQualityMonitorService.stopMonitoring()
      end if
    end function

'----------------------------------------
' Constructors
'----------------------------------------

    link: function(view) as Object
      m.view = view
      m.videoDataService = VideoDataService()
      m.heartbeatService = m.registerDelegate(HeartbeatService())
      m.view.addEventListener(m, m.view.HEARTBEAT, "handleHeartbeat")
      m.view.addEventListener(m, m.view.VIDEO_COMPLETE, "handleVideoComplete")
      m.view.addEventListener(m, m.view.VIDEO_LOADED, "handleVideoLoaded")
      m.view.addEventListener(m, m.view.BOOKMARK, "handleBookmark")
      return m
    end function

    init: function() as Void
      m.xhr.bypassMaintenanceMode = true
    end function

  })
  obj.typeof = obj.typeof + ".VideoPlayerController"
  return obj
end function
