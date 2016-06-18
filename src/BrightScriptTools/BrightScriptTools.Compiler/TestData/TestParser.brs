'///////////////////////////////////////////////////////////////
'//
'//  $copyright: Copyright (C) 2015 BSKYB
'//
'///////////////////////////////////////////////////////////////
function ButtonFactory() as Object
    return ObjectUtils().createSingleton("ButtonFactory", {

'----------------------------------------
' Dependencies
'----------------------------------------
  stringUtils: StringUtils()
  ImageTextButton: EventImageTextButton

'----------------------------------------
' Public API
'----------------------------------------

    createButton: function(buttonProperties as Object, optionalPrefixText = "" as String, optionalSuffixText = "" as String, style= StyleUtils().ImageTextButton as Object, optionalPrefixTextAsBold = false, optionalSuffixTextAsBold = false ) as Object
      if not m.stringUtils.isInvalidOrWhiteSpace(optionalPrefixText)
        optionalPrefixText = optionalPrefixText +  " "
      end if

      if not m.stringUtils.isInvalidOrWhiteSpace(optionalSuffixText)
        optionalSuffixText = optionalSuffixText +  " "
      end if

      if buttonProperties.style <> Invalid
        style = buttonProperties.style
      end if

      newButton = m.ImageTextButton(buttonProperties.id, style)
      newButton.initSurface(style.width, style.height)
      font = Invalid

      if not m.stringUtils.isInvalidOrWhiteSpace(optionalPrefixText)
        if optionalPrefixTextAsBold 
          font = style.bold.font 
        else
          font = style.text.font 
        end if
        newButton.addChild(optionalPrefixText, font, "")
      end if

      newButton.addChild(buttonProperties.text, style.text.font, "")

      if not m.stringUtils.isInvalidOrWhiteSpace(optionalSuffixText)
        if optionalSuffixTextAsBold 
          font = style.bold.font 
        else
          font = style.text.font 
        end if
        newButton.addChild(optionalSuffixText, font, "")
      end if

      buttonProperties.button = newButton
      buttonProperties.visible = true
      newButton.update()
      return newButton
    end function  

  getImageTextButton: function(id as String, title as String, style=StyleUtils().imageTextButtonXL as Object) as Object
    btn = m.ImageTextButton(id,style)
    btn.initSurface(style.width, style.height)
    if(style.images <> Invalid)
      if(style.images.activeImageFile <> Invalid)
	      btn.activeImageFile = style.images.activeImageFile
      end if
      if(style.images.inActiveImageFile <> Invalid)
        btn.inActiveImageFile = style.images.inActiveImageFile
      end if
      if(style.images.pressedImageFile <> Invalid)
        btn.pressedImageFile = style.images.pressedImageFile
      end if
    end if
    btn.setText(title)
    return btn
  end function

 	getTextOnlyXXLButton: function(id as String, title as String, style=StyleUtils().textOnlyButtonXXL as Object) as Object
    return m.getImageTextButton(id, title, style)
	end function

	getXXLButton: function(id as String, title as String, style=StyleUtils().imageTextButtonXXL as Object) as Object
    return m.getImageTextButton(id, title, style)
	end function

	getXXXLButton: function(id as String, title as String, style=StyleUtils().imageTextButtonXXXL as Object) as Object
    return m.getImageTextButton(id, title, style)
  end function

  getXLButton: function(id as String, title as String, style=StyleUtils().imageTextButtonXL as Object) as Object
    return m.getImageTextButton(id, title, style)
  end function

  getXSButton: function(id as String, title as String, style=StyleUtils().imageTextButtonXS as Object) as Object
    return m.getImageTextButton(id, title, style)
  end function

})
end function
