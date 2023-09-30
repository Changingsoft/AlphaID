(function () {
    $.validator.setDefaults({
        // only validate fields when the form is submitted:
        // the Captcha input must only be validated when the whole code string is
        // typed in, not after each individual character (onkeyup must be false);
        // onfocusout validation could be left on in more complex forms, but 
        // doesn't fit this example
        onkeyup: false,
        onfocusout: false,
        // always reload the Captcha image if remote validation failed,
        // since it will not be usable any more (a failed validation attempt
        // removes the attempted code for necessary Captcha security
        showErrors: function (errorMap, errorList) {
            for (var i = 0; i < errorList.length; i++) {
                var element = errorList[i].element;
                var message = errorList[i].message;
                // check element css class and does the error message match remote
                // validation failure
                if (element.className.match(/captchaVal/)) {
                    var captchaCodeInputProperty = 'Input.' + element.id;
                    if (message === this.settings.messages[captchaCodeInputProperty].remote) {
                        element.Captcha.ReloadImage();
                        $("form").valid();
                    }
                }
            }
        }
    });
})();


$(document).ready(function () {
    // add validation rules by CSS class, so we don't have to know the
    // exact client id of the Captcha code textbox
    $(".captchaVal").rules('add', {
        required: true,
        remote: $(".captchaVal").get(0).Captcha.ValidationUrl,
        messages: {
            required: "Your input doesn't match displayed characters.",
            remote: "Incorrect code, please try again."
        }
    });
});