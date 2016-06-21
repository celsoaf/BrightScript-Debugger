'///////////////////////////////////////////////////////////////
'//
'//  $copyright: Copyright (C) 2013 BSKYB
'//
'///////////////////////////////////////////////////////////////
function AppController() as Object
  return ObjectUtils().extend("AppController", CoreController(), {

'----------------------------------------
' Dependencies
'----------------------------------------

    ErrorView: Invalid
    GenreView: Invalid
    SignInSplashView: Invalid
    SignInPasswordView: Invalid
    SignInUsernameView: Invalid
    ProgrammeDetailsView: Invalid
    CompositeView: Invalid
    BoxSetDetailsView: Invalid
    MyLibraryView: Invalid
    SearchInputView: Invalid
    SearchResultsView: Invalid
    VideoPlayerView: Invalid
    MyAccountView: Invalid
    LaunchView: Invalid
    HelpView: Invalid
    HelpItemView: Invalid
    MaintenanceView: Invalid
    ConfirmParentalPinSubView: Invalid
    MessageView: Invalid
    DeveloperAdminView: Invalid
    DeveloperInfoView: Invalid
    ListSelectView: Invalid
    DeveloperUserActionsView: Invalid

    rt: Routes()
    StateVO: StateVO
    logger: Logger()

'----------------------------------------
' Constants
'----------------------------------------

    r: "route:"

'----------------------------------------
' Public API
'----------------------------------------

    initViews: function() as Void
      m.ErrorView = ErrorView
      m.GenreView = GenreView
      m.SignInSplashView = SignInSplashView
      m.SignInCaptchaView = SignInCaptchaView
      m.SignInPasswordView = SignInPasswordView
      m.SignInUsernameView = SignInUsernameView
      m.ProgrammeDetailsView = ProgrammeDetailsView
      m.CompositeView = CompositeView
      m.BoxSetDetailsView = BoxSetDetailsView
      m.MyLibraryView = MyLibraryView
      m.SearchInputView = SearchInputView
      m.SearchResultsView = SearchResultsView
      m.VideoPlayerView = VideoPlayerView
      m.MyAccountView = MyAccountView
      m.LaunchView = LaunchView
      m.HelpView = HelpView
      m.HelpItemView = HelpItemView
      m.MaintenanceView = MaintenanceView
      m.AboutSkyStoreView = AboutSkyStoreView
      m.FranchiseView = FranchiseView
      m.MessageView = MessageView
      m.AddressSelectDeliveryView = AddressSelectDeliveryView
      m.AddressChangeCountryView = AddressChangeCountryView
      m.AddressSelectCountryView = AddressSelectCountryView
      m.AddressSelectMethodView = AddressSelectMethodView
      m.AddressEnterPostcodeView = AddressEnterPostcodeView
      m.AddressDeliveryManualEntryView = AddressDeliveryManualEntryView
      m.AddressSelectDeliveryForPostcodeView = AddressSelectDeliveryForPostcodeView
      m.DeveloperAdminView = DeveloperAdminView
      m.DeveloperInfoView = DeveloperInfoView
      m.ListSelectView = ListSelectView
      m.DeveloperUserActionsView = DeveloperUserActionsView
    end function

'----------------------------------------
' Private API
'----------------------------------------

    observeEvents: function() as Void
      m.messageBus.addEventListener(m, m.r + m.rt.LAUNCH, "showLaunch")
      m.messageBus.addEventListener(m, m.r + m.rt.SHOWCASE, "showShowcaseView")
      m.messageBus.addEventListener(m, m.r + m.rt.PROGRAMME_DETAILS, "showProgrammeDetailsView")
      m.messageBus.addEventListener(m, m.r + m.rt.BOXSET_DETAILS, "showBoxSetDetailsView")
      m.messageBus.addEventListener(m, m.r + m.rt.FRANCHISE_VIEW, "showFranchiseView")
      m.messageBus.addEventListener(m, m.r + m.rt.SEASON_DETAILS, "showSeasonDetailsSubView")
      m.messageBus.addEventListener(m, m.r + m.rt.GENRE, "showGenreView")
      m.messageBus.addEventListener(m, m.r + m.rt.MY_LIBRARY, "showMyLibraryView")
      m.messageBus.addEventListener(m, m.r + m.rt.SEARCH_INPUT, "showSearchInputView")
      m.messageBus.addEventListener(m, m.r + m.rt.SEARCH_RESULTS, "showSearchResultsView")
      m.messageBus.addEventListener(m, m.r + m.rt.VIDEO_PLAYER, "showVideoPlayerView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGN_IN_SPLASH, "showSignInSplashView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGN_IN_USERNAME, "showSignInUsernameView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGN_IN_PASSWORD, "showSignInPasswordView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGN_IN_CAPTCHA, "showSignInCaptchaView")
      m.messageBus.addEventListener(m, m.r + m.rt.MY_ACCOUNT, "showMyAccount")
      m.messageBus.addEventListener(m, m.r + m.rt.HELP, "showHelpView")
      m.messageBus.addEventListener(m, m.r + m.rt.HELP_ITEM, "showHelpItemView")
      m.messageBus.addEventListener(m, m.r + m.rt.ORDER_SUMMARY, "showOrderSummaryView")
      m.messageBus.addEventListener(m, m.r + m.rt.ORDER_ADDRESS, "showOrderAddressView")
      m.messageBus.addEventListener(m, m.r + m.rt.ORDER_CONFIRMATION, "showOrderConfirmationSubView")
      m.messageBus.addEventListener(m, m.r + m.rt.TRANSACT_PIN, "showTransactPinView")
      m.messageBus.addEventListener(m, m.r + m.rt.PARENTAL_PIN, "showConfirmParentalPinSubView")
      m.messageBus.addEventListener(m, m.r + m.rt.ABOUT_SKY_STORE, "showAboutSkyStoreView")
      m.messageBus.addEventListener(m, m.r + m.rt.ADDRESS_SELECT_DELIVERY, "showAddressSelectDeliveryView")
      m.messageBus.addEventListener(m, m.r + m.rt.ADDRESS_SELECT_COUNTRY, "showAddressSelectCountryView")
      m.messageBus.addEventListener(m, m.r + m.rt.ADDRESS_CHANGE_COUNTRY, "showAddressChangeCountryView")
      m.messageBus.addEventListener(m, m.r + m.rt.ADDRESS_SELECT_METHOD, "showAddressSelectMethodView")
      m.messageBus.addEventListener(m, m.r + m.rt.ADDRESS_ENTER_POSTCODE, "showAddressEnterPostcodeView")
      m.messageBus.addEventListener(m, m.r + m.rt.ADDRESS_SELECT_DELIVERY_FOR_POSTCODE, "showAddressSelectDeliveryForPostcodeView")
      m.messageBus.addEventListener(m, m.r + m.rt.ADDRESS_DELIVERY_MANUAL_EDIT, "showAddressDeliveryManualEditView")
      m.messageBus.addEventListener(m, m.r + m.rt.ADDRESS_DELIVERY_MANUAL_ADD, "showAddressDeliveryManualAddView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGNUP_WELCOME, "showSignUpWelcomeView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGNUP_TERMS, "showSignUpTermsView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGNUP_PAYMENT_DETAILS, "showSignUpPaymentDetailsView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGNUP_PIN, "showSignUpPinView")
      m.messageBus.addEventListener(m, m.r + m.rt.SIGNUP_CONFIRMATION, "showSignUpConfirmationView")
      m.messageBus.addEventListener(m, m.r + m.rt.MAINTENANCE, "showMaintenanceView")
      m.messageBus.addEventListener(m, m.r + m.rt.ERROR, "showErrorView")
      m.messageBus.addEventListener(m, m.r + m.rt.MESSAGE, "showMessageView")
      m.messageBus.addEventListener(m, m.r + m.rt.DEVELOPER_ADMIN, "showDeveloperAdminView")
      m.messageBus.addEventListener(m, m.r + m.rt.DEVELOPER_INFO, "showDeveloperInfoView")
      m.messageBus.addEventListener(m, m.r + m.rt.LIST_SELECT, "showListSelectView")
      m.messageBus.addEventListener(m, m.r + m.rt.DEVELOPER_USER_ACTIONS_SELECT, "showDeveloperUserActionsSelect")
    end function

    showContentView: function(view as Object, state as Object) as Void
      m.application.showContentView(view, state)
      view.postInit()

      if state.route <> Invalid
        m.logger.info("Navigate to '" + state.route + "'")
      end if
      ExecutionService().execute(TrackMenuAnalytics(), "pageLoad", [state])
    end function

    showContentSubView: function(view as Object, state=Invalid as Object, controller=Invalid as Object) as Void
      m.application.showContentSubView(view, state, controller)

      if state <> Invalid
        m.logger.info("Navigate to sub '" + state.route + "'")
      end if
    end function

'----------------------------------------
' Handlers
'----------------------------------------

    showHelpItemView: function(eventObj as Object) as Void
      m.showContentView(m.HelpItemView(), eventObj.state)
    end function

    showAboutSkyStoreView: function(eventObj as Object) as Void
      m.showContentView(m.AboutSkyStoreView(), eventObj.state)
    end function

    showHelpView: function(eventObj as Object) as Void
      m.showContentView(m.HelpView(), eventObj.state)
    end function

    showMaintenanceView: function(eventObj as Object) as Void
      m.showContentView(m.MaintenanceView(), eventObj.state)
    end function

    showLaunch: function(eventObj as Object) as Void
      m.showContentView(m.LaunchView(), eventObj.state)
    end function

    showErrorView: function(eventObj as Object) as Void
      m.showContentView(m.ErrorView(), eventObj.state)
    end function

    showGenreView: function(eventObj as Object) as Void
      m.showContentView(m.GenreView(), eventObj.state)
    end function

    showShowcaseView: function (eventObj as Object) as Void
      m.showContentView(m.GenreView(), eventObj.state)
    end function

    showSignInCaptchaView: function (eventObj as Object) as Void
      m.showContentView(m.SignInCaptchaView(), eventObj.state)
    end function

    showSignInSplashView: function (eventObj as Object) as Void
      m.showContentView(m.SignInSplashView(), eventObj.state)
    end function

    showSignInPasswordView: function(eventObj as Object) as Void
      m.showContentView(m.SignInPasswordView(), eventObj.state)
    end function

    showMyAccount: function(eventObj as Object) as Void
      m.showContentView(m.MyAccountView(), eventObj.state)
    end function

    showSignInUsernameView: function(eventObj as Object) as Void
      m.showContentView(m.SignInUsernameView(), eventObj.state)
    end function

    showProgrammeDetailsView: function(eventObj as Object) as Void
      m.showContentView(m.ProgrammeDetailsView(), eventObj.state)
    end function

    showFranchiseView: function (eventObj as Object) as Void
      m.showContentView(m.FranchiseView(), eventObj.state)
    end function

    showBoxSetDetailsView: function(eventObj as Object) as Void
      m.showContentView(m.BoxSetDetailsView(), eventObj.state)
    end function

    showMyLibraryView: function(eventObj as Object) as Void
      m.showContentView(m.MyLibraryView(), eventObj.state)
    end function

    showSearchInputView: function(eventObj as Object) as Void
      m.showContentView(m.SearchInputView(), eventObj.state)
    end function

    showSearchResultsView: function(eventObj as Object) as Void
      m.showContentView(m.SearchResultsView(), eventObj.state)
    end function

    showVideoPlayerView: function(eventObj as Object) as Void
      m.showContentView(m.VideoPlayerView(), eventObj.state)
    end function

    showOrderSummaryView: function(eventObj as Object) as Void
      st = eventObj.state 
      st.subView = OrderSummarySubView()
      st.subController = OrderSummaryController 
      m.showContentSubView(m[st.viewID](), st, OrderSummaryController)
    end function

    showOrderAddressView: function(eventObj as Object) as Void
      st = eventObj.state 
      st.subView = OrderAddressSubView()
      st.subController = OrderAddressController
      m.showContentSubView(m[st.viewID](), st, OrderAddressController)
    end function    

    showTransactPinView: function(eventObj as Object) as Void
      st = eventObj.state 
      st.subView = ConfirmPurchasePinSubView("ConfirmPurchasePinSubView")
      m.showContentSubView(m[st.viewID](), st, ConfirmPurchasePinController)
    end function

    showOrderConfirmationSubView: function(eventObj as Object) as Void
      st = eventObj.state
      st.subView = OrderConfirmationSubView()
      m.showContentSubView(m[st.viewID](), st, OrderConfirmationController)
    end function

    showConfirmParentalPinSubView: function(eventObj as Object) as Void
      st = eventObj.state
      st.subView = ConfirmParentalPinSubView()
      m.showContentSubView(m[st.viewID](), st, ConfirmParentalPinController)
    end function

    showAddressSelectDeliveryView: function(eventObj as Object) as Void
      m.showContentView(m.AddressSelectDeliveryView(), eventObj.state)
    end function

    showAddressSelectCountryView: function(eventObj as Object) as Void
      m.showContentView(m.AddressSelectCountryView(), eventObj.state)
    end function

    showAddressChangeCountryView: function(eventObj as Object) as Void
      m.showContentView(m.AddressChangeCountryView(), eventObj.state)
    end function

    showAddressSelectMethodView: function(eventObj as Object) as Void
      m.showContentView(m.AddressSelectMethodView(), eventObj.state)
    end function

    showAddressEnterPostcodeView: function(eventObj as Object) as Void
      m.showContentView(m.AddressEnterPostcodeView(), eventObj.state)
    end function

    showAddressSelectDeliveryForPostcodeView: function(eventObj as Object) as Void
      m.showContentView(m.AddressSelectDeliveryForPostcodeView(), eventObj.state)
    end function

    showAddressDeliveryManualEditView: function(eventObj as Object) as Void
      controller = AddressDeliveryManualEditController()
      m.showContentView(m.AddressDeliveryManualEntryView("AddressDeliveryManualEditView", controller), eventObj.state)
    end function

    showAddressDeliveryManualAddView: function(eventObj as Object) as Void
      controller = AddressDeliveryManualAddController()
      m.showContentView(m.AddressDeliveryManualEntryView("AddressDeliveryManualAddView", controller), eventObj.state)
    end function

    showSignUpWelcomeView: function(eventObj as Object) as Void
      m.showContentView(SignUpWelcomeView(), eventObj.state)
    end function

    showSignUpTermsView: function(eventObj as Object) as Void
      m.showContentView(SignUpTermsView(), eventObj.state)
    end function

    showSignUpPaymentDetailsView: function(eventObj as Object) as Void
      m.showContentView(SignUpPaymentDetailsView(), eventObj.state)
    end function

    showSignUpPinView: function(eventObj as Object) as Void
      m.showContentView(SignUpPinView(), eventObj.state)
    end function

    showSignUpConfirmationView: function(eventObj as Object) as Void
      m.showContentView(SignUpConfirmationView(), eventObj.state)
    end function

    showMessageView: function(eventObj as Object) as Void
      m.showContentView(m.MessageView(), eventObj.state)
    end function

    showDeveloperAdminView: function(eventObj as Object) as Void
      m.showContentView(m.DeveloperAdminView(), eventObj.state)
    end function

    showDeveloperInfoView: function(eventObj as Object) as Void
      m.showContentView(m.DeveloperInfoView(), eventObj.state)
    end function

    showListSelectView: function(eventObj as Object) as Void
      m.showContentView(m.ListSelectView(), eventObj.state)
    end function

    showDeveloperUserActionsSelect: function(eventObj as Object) as Void
      m.showContentView(m.DeveloperUserActionsView(), eventObj.state)
    end function
'----------------------------------------
' Constructors
'----------------------------------------

    link: function(view as Object) as Object
      m.application = view
      m.observeEvents()
      return m
    end function
  })
  this.typeof = this.typeof + ".AppController"
  return this
end function
