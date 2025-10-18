
$(document).off('keypress', "#LotNo");
$(document).on('keypress', "#LotNo", function (e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        let lotNo = $.trim($('#LotNo').val());
        GetTransactions(lotNo);
        return false;
    }
   
});


$(document).off('click', "#btnNextLotNo");
$(document).on('click', "#btnNextLotNo", function (e) {
    e.preventDefault();
    let lotNo = $.trim($('#LotNo').val());
    GetTransactions(lotNo);
});




