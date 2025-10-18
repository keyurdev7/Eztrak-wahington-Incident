$("body").on("keyup keydown", '#Password', function () {
    var password = $(this).val();
    var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@@#$%^&*()_+}{"':;?/>.<,])(?=.{9,})/;
    if (regex.test(password)) {
        // Password meets requirements
        console.log("Password is valid");
        $("div.form-info").addClass("valid");
        $("div.form-info p i").removeClass("fa-xmark").addClass("fa-check");

    } else {
        // Password does not meet requirements
        console.log("Password is invalid");
        $("div.form-info").removeClass("valid");
        $("div.form-info p i").removeClass("fa-check").addClass("fa-xmark");
    }
});