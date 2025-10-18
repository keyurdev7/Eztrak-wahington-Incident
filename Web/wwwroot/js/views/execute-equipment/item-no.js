
$(document).off('keypress', "#ItemNo");
$(document).on('keypress', "#ItemNo", function (e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        let lotNo = $.trim($('#ItemNo').val());
        GetTransactions(lotNo);
        return false;
    }
   
});

$(document).off('click', "#btnNextItemNo");
$(document).on('click', "#btnNextItemNo", function (e) {
    e.preventDefault();
    let itemNo = $.trim($('#ItemNo').val());
    GetTransactions(itemNo);
});




