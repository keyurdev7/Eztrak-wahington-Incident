var backBtn = $("#nwoBackBtn");
var nextBtn = $("#nwoNextBtn");
var saveBtn = $("#nwoSaveBtn");
var allSteps = $(".nwoStep");
var stepCount = 0;

$(function () {
    setPageNumber();
    if ($(".nwoStep").length > 0) {
        $(".nwoStep").first().removeClass("d-none");
    }
})

function handleStepChange(increment) {
    $(allSteps[stepCount]).addClass("d-none");
    stepCount += increment;
    $(allSteps[stepCount]).removeClass("d-none");
    updateBtnDisplay();
    setPageNumber();
}

nextBtn.on("click", function (event) {
    if (validateCurrentStep()) {
        handleStepChange(1);
    }
});

backBtn.on("click", function (event) {
    handleStepChange(-1);
    nextBtn.removeAttr("disabled");
});

function setPageNumber() {
    $(".nwoStep:not(.d-none)").find(".page-number").html(`${stepCount + 1}/${allSteps.length}`)
}

function updateBtnDisplay() {
    if (stepCount === allSteps.length - 1) {
        nextBtn.addClass("d-none");
        backBtn.removeClass("d-none");
        saveBtn.removeClass("d-none");
    } else if (stepCount == 0) {
        nextBtn.removeClass("d-none");
        backBtn.addClass("d-none");
        saveBtn.addClass("d-none");
    } else {
        nextBtn.removeClass("d-none");
        backBtn.removeClass("d-none");
        saveBtn.addClass("d-none");
    }
}

function validateCurrentStep() {
    // Select only the visible input fields within the current step
    var visibleInputs = $('.nwoStep:visible').find(':input:visible');

    // Validate the visible input fields
    var isValid = true;
    visibleInputs.each(function () {
        if (!$(this).valid()) {
            isValid = false;
        }
    });

    return isValid;
}
