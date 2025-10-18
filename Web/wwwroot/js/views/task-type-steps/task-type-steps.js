var drawingFileListData = [];

$(function () {

    $(document).off("click", ".add-step");
    $(document).on("click", ".add-step", function () {
        AddStep(this);
    });
    $(document).off("click", ".remove-step");
    $(document).on("click", ".remove-step", function () {
        $(this).closest("tr").remove();
        RefreshInputIndexes("#step-table tr", "step-section-row", "Steps");
    });

});

function AddStep(element) {
    var thisElement = $(element);
    var jobType = $("#step-job-type").val();
    var men = $("#step-men").val();
    var duration = $("#step-duration").val();
    removeCurrencyMasking();
    var material = $("#step-material").val();
    var parsedMaterial = parseFloat(material.replace('$', ''));

    var equipment = $("#step-equipment").val();
    var parsedEquipment = parseFloat(equipment.replace('$', ''));

    var costCraftId = $('#step-craftskill-id').select2('data')[0].id;
    var costCraftName = $('#step-craftskill-id').select2('data')[0].text;
    if (jobType !== null && jobType !== "" && duration != "" && duration > 0 && men != "" && men > 0 && costCraftId != null && costCraftId != "" && equipment != "" && equipment != null && material != "" && material != null) {
        var rowNumber = $(".step-section-row").length;
        var obj = {
            'JobType': jobType,
            'CraftSkill': {
                'Id': costCraftId,
                'Name': costCraftName,
                'STRate': $("#step-craftskill-id").select2('data')[0].additionalAttributesModel.STRate,
                'OTRate': $("#step-craftskill-id").select2('data')[0].additionalAttributesModel.OTRate,
                'DTRate': $("#step-craftskill-id").select2('data')[0].additionalAttributesModel.DTRate,
            },
            'Men': men,
            'Hours': duration,
            'Material': material,
            'Equipment': equipment
        }
        AddGridRow(obj, rowNumber, "/TaskType/_TaskStepRow", "step-table tbody");
        ResetInputs([".step-container"], ["#step-craftskill-id"]);
        addCurrencyMasking();
    }
    else {
        FireSwal("JobType, Craft, Men, Duration, Equipment & Material are required fields.");
    }
}

function FireSwal(errorMessage) {
    swal.fire({
        title: 'Validation Error',
        text: errorMessage,
        type: 'error',
        showCancelButton: true,
        confirmButtonText: "Okay",
        cancelButtonText: "Cancel",
        confirmButtonClass: 'btn btn-success m-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    });
}

function AddGridRow(obj, rowNumber, url, tbodyId, rowClass = '', collectionType = '') {
    $.ajax({
        url: url,
        type: "post",
        data: { model: obj, 'rowNumber': rowNumber, 'rowClass': rowClass, 'collectionType': collectionType },
        dataType: "html",
        success: function (response) {
            $('#' + tbodyId).append(response);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
        }
    });
}

function ResetInputs(containers, select2Elements) {
    //to reset container elements
    $.each(containers, function (index, selector) {
        $(selector).find(':input').val(null);
    });
    //to reset select 2 elements
    $.each(select2Elements, function (index, selector) {
        $(selector).val('').trigger('change')
    });
}

function RefreshInputIndexes(trSelector, rowPrefix, collectionName) {
    var rowIndex = 0;
    $.each($(trSelector), function () {

        if ($(this).find('input').length > 1) {
            var id = $(this).attr("id");
            if (id != null && id != "") {
                $(this).attr("id", rowPrefix + "-" + rowIndex);
            }
            $.each($(this).find('input'), function () {
                if ($(this).attr('name').split(".").length > 2) {

                    $(this).attr("name", collectionName + "[" + rowIndex + "]." + $(this).attr('name').split(".")[1] + "." + $(this).attr('name').split(".")[2]);
                }
                else {
                    $(this).attr("name", collectionName + "[" + rowIndex + "]." + $(this).attr('name').split(".")[1]);
                }
            });
            rowIndex++;
        }
    });
}

function removeCurrencyMasking() {
    $(".input-currency").each(function (index, element) {
        $(element).inputmask("remove");
        $(element).attr("data-val", "true");
    });
}
function addCurrencyMasking() {
    maskCurrency(".input-currency");
}