function onDropdownChangeResetDependentDropdown(id, dependentDropdowns) {
    $(id).on("change", function (e) {
        for (var i = 0; i < dependentDropdowns.length; i++) {
            var dependentSelect2Ids = [];
            $('[id*=' + dependentDropdowns[i] + ']').each((index, element) => {
                if ($(element).data('select2')) {
                    dependentSelect2Ids.push($(element).attr('id'));
                }
            });
            for (var j = 0; j < dependentSelect2Ids.length; j++) {
                $("#" + dependentSelect2Ids[j]).val(null).trigger('change');
            }
        }
    });
}