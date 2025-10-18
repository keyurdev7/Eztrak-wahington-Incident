$(function () {
    //setting default for repair and replace
    //setRepairAndReplace();

    $("body").off("change", "#work-order-task")
    $("body").on("change", "#work-order-task", function () {
        console.log($(this).val());
        setRepairAndReplace(this);
    });

    $(document).off("click", ".add-cost");
    $(document).on("click", ".add-cost", function () {
        AddCost(this);

    });
    $(document).off("click", ".remove-cost");
    $(document).on("click", ".remove-cost", function () {
        $(this).closest("tr").remove();
        RefreshInputIndexes("#labour-table tr", "labour-section-row", "WorkOrderLabors");
    });

    $(document).off("click", ".add-material");
    $(document).on("click", ".add-material", function () {
        AddMaterial(this);

    });
    $(document).off("click", ".remove-material");
    $(document).on("click", ".remove-material", function () {
        $(this).closest("tr").remove();
        RefreshInputIndexes("#material-table tr", "material-section-row", "WorkOrderMaterials");
    });

    $(document).off("click", ".add-equipment");
    $(document).on("click", ".add-equipment", function () {
        AddEquipment(this);

    });
    $(document).off("click", ".remove-equipment");
    $(document).on("click", ".remove-equipment", function () {
        $(this).closest("tr").remove();
        RefreshInputIndexes("#equipment-table tr", "equipment-section-row", "WorkOrderEquipments");
    });

    $(document).off("click", ".add-technician");
    $(document).on("click", ".add-technician", function () {
        AddTechnician(this);

    });
    $(document).off("click", ".remove-technician");
    $(document).on("click", ".remove-technician", function () {
        $(this).closest("tr").remove();
        RefreshInputIndexes("#technician-table tr", "technician-section-row", "WorkOrderTechnicians");
    });

});

function setRepairAndReplace(element) {
    //$(".task-type-label").html($(element).find("option:selected").text());
    $(".task-type-label").html('Work Step<span style="color:red"> *</span>');
    $("#task-type-id").val(null).trigger('change');
}

function AddCost(element) {
    var thisElement = $(element);
    var overrideType = $("#labour-override-type").val();
    var costHours = $("#labour-hours").val();
    var headCount = $("#head-count").val();
    var costCraftId = $('#craft-skill-id').select2('data')[0].id;
    var costCraftName = $('#craft-skill-id').select2('data')[0].text;
    if (overrideType !== null && overrideType !== "" && costHours != null && headCount != "" && headCount > 0 && costHours != "" && costHours > 0 && costCraftId != null && costCraftId != "") {
        var rowNumber = $(".labour-section-row").length;
        var obj = {
            'Type': overrideType,
            'Craft': {
                'Id': costCraftId,
                'Name': costCraftName,
                'STRate': $("#craft-skill-id").select2('data')[0].additionalAttributesModel.STRate,
                'OTRate': $("#craft-skill-id").select2('data')[0].additionalAttributesModel.OTRate,
                'DTRate': $("#craft-skill-id").select2('data')[0].additionalAttributesModel.DTRate,
            },
            'MN': headCount,
            'DU': costHours
        }
        AddGridRow(obj, rowNumber, "/WorkOrder/_LabourSectionRow", "labour-table tbody");
        ResetInputs([".costs-inputs-container"], ["#craft-skill-id"]);
    }
    else {
        FireSwal("Type, Craft, Head Count & Hours are required fields.");
    }
}

function AddMaterial(element) {
    var thisElement = $(element);
    var quantity = $("#quantity").val();
    var inventoryId = $('#inventory-id').select2('data')[0].id;
    var inventoryName = $('#inventory-id').select2('data')[0].text;
    if (quantity != null && quantity > 0 && inventoryId != null && inventoryId != "") {
        var rowNumber = $(".material-section-row").length;
        var obj = {
            'Quantity': quantity,
            'Inventory': {
                'Id': inventoryId,
                'ItemNo': inventoryName,
            }
        }
        AddGridRow(obj, rowNumber, "/WorkOrder/_MaterialSectionRow", "material-table tbody");
        ResetInputs([".material-inputs-container"], ["#inventory-id"]);
    }
    else {
        FireSwal("Quantity & Material are required fields.");
    }
}

function AddEquipment(element) {
    var thisElement = $(element);
    var quantity = $("#equipment-quantity").val();
    var equipment = $('#equipment-id').select2('data')[0].id;
    var equipmentName = $('#equipment-id').select2('data')[0].text;
    var hours = $("#equipment-hours").val();
    if (quantity != null && quantity > 0 && equipment != null && equipment != "") {
        var rowNumber = $(".equipment-section-row").length;
        var obj = {
            'Quantity': quantity,
            'Hours': hours,
            'Equipment': {
                'Id': equipment,
                'Description': equipmentName,
            }
        }
        AddGridRow(obj, rowNumber, "/WorkOrder/_EquipmentSectionRow", "equipment-table tbody");
        ResetInputs([".equipment-inputs-container"], ["#equipment-id"]);
    }
    else {
        FireSwal("Quantity, Hours & Equipment are required fields.");
    }
}

function AddTechnician(element) {
    var thisElement = $(element);
    var technicianId = $('#technician-id').select2('data')[0].id;
    var technicianName = $('#technician-id').select2('data')[0].text;
    var craftskillId = $('#craftskill-id').select2('data')[0].id;
    var craftskillName = $('#craftskill-id').select2('data')[0].text;
    if (craftskillId != null && craftskillId != "" && craftskillId != 0 && technicianId != null && technicianId != "" && technicianId != 0) {
        var rowNumber = $(".technician-section-row").length;
        var obj = {
            'Technician': {
                'Id': technicianId,
                'Name': technicianName,
            },
            'CraftSkill': {
                'Id': craftskillId,
                'Name': craftskillName,
            }
        }
        AddGridRow(obj, rowNumber, "/WorkOrder/_TechnicianSectionRow", "technician-table tbody");
        ResetInputs([".technician-inputs-container"], ["#technician-id", "#craftskill-id"]);
    }
    else {
        FireSwal("Technician & CraftSkill are required fields.");
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