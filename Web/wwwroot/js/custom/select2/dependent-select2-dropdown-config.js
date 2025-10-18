var administrativeDivisionTypes = ["country", "province", "division", "district", "sub-division", "tehsil", "union-council", "village", "sub-village"];
function onAdministrativeDivisionTypeChange(id, name) {
    var divisionTypeIndex = administrativeDivisionTypes.findIndex(x => x === name);
    if (divisionTypeIndex > -1) {
        var divisionType = administrativeDivisionTypes[divisionTypeIndex];
        var childDivisionType = administrativeDivisionTypes[divisionTypeIndex + 1];
        var childDivisionTypeId = id.replace(divisionType, childDivisionType);
        $(id).on("change", function (e) {
            if ($(childDivisionTypeId).length > 0) {
                $(childDivisionTypeId).val(null).trigger('change');
            }
        });
    }
}
