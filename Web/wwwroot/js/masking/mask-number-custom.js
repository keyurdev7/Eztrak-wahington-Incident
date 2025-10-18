function maskTelephone(identifier) {
    $(identifier).inputmask('999-999-9999', { placeholder: 'xxx-xxx-xxxx', removeMaskOnSubmit: true, rightAlign: false });
}
function maskDatatableTelephone(identifier, tableIdentifier) {
    $(tableIdentifier).find(identifier).inputmask('999-999-9999');
}

function maskCurrency(identifier) {
    $(identifier).inputmask({ alias: "currency", prefix: '$', removeMaskOnSubmit: true, rightAlign: false });
    $(".input-currency").each(function (index, element) {
        $(element).attr("data-val", "false");
    });
}
function maskDatatableCurrency(identifier, tableIdentifier) {
    $(tableIdentifier).find(identifier).inputmask({ alias: "currency", prefix: '$', removeMaskOnSubmit: true, rightAlign: false });
}


