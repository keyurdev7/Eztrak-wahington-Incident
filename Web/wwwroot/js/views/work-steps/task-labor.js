$(function () {
    $('.unsaved-labor-error').hide();

    $(document).off('click', '#addLabor');
    $(document).on('click', '#addLabor', function () {
        if (hasUnsavedLaborRows()) {
            $('.unsaved-labor-error').show();
            return;
        }

        $('.unsaved-labor-error').hide();
        const newRow = `
                        <tr>
                            <td><input type="text" class="laborInput form-control" data-name="Title"></td>
                            <td><input type="number" class="hoursInput form-control hours" data-name="Hours"></td>
                            <td><input type="number" class="rateInput form-control" step="0.01" data-name="Rate"></td>
                            <td class="total"></td>
                            <td>
                                <span class="icon save-labor"><i class="icon-floppy-disk"></i></span>
                                <span class="icon delete-labor"><i class="icon-trash"></i></span>
                            </td>
                        </tr>
                    `;
        $('#laborTable tbody').append(newRow);
        refreshLaborIndexes();
    });

    $(document).off('click', '.save-labor');
    $(document).on('click', '.save-labor', function () {
        const row = $(this).closest('tr');
        const laborInput = row.find('.laborInput');
        const hoursInput = row.find('.hoursInput');
        const rateInput = row.find('.rateInput');
        const errorDiv = row.find('.error');

        let isValid = CheckLaborValidation(laborInput, hoursInput, rateInput);

        if (isValid) {
            const labor = laborInput.val();
            const hours = parseFloat(hoursInput.val());
            const rate = parseFloat(rateInput.val());
            const total = hours * rate;

            row.html(`
                        <td>
                            ${labor}
                            <input type="hidden" name="${laborInput.attr('name')}" value="${labor}"  data-name="Title">
                        </td>
                        <td>
                            ${hours}
                            <input type="hidden" class="hours" name="${hoursInput.attr('name')}" value="${hours}" data-name="Hours">
                        </td>
                        <td>
                            $${rate.toFixed(2)}
                            <input type="hidden" name="${rateInput.attr('name')}" value="${rate}" data-name="Rate">
                        </td>
                        <td>$<span class="cost">${total.toFixed(2)}</span></td>
                        <td>
                            <span class="icon edit-labor"><i class="icon-pencil5"></i></span>
                            <span class="icon delete-labor"><i class="icon-trash"></i></span>
                        </td>
                    `);
            refreshLaborIndexes();
            updateTotalHours();
            updateTotalCost();

        } else {
            errorDiv.show();
        }


    });

    $(document).off('click', '.edit-labor');
    $(document).on('click', '.edit-labor', function () {
        const row = $(this).closest('tr');
        const labor = row.children('td').eq(0).text().trim();
        const hours = row.children('td').eq(1).text().trim();
        const rate = row.children('td').eq(2).text().replace('$', '').trim();

        row.html(`
                    <td><input type="text" class="laborInput form-control" data-name="Title" value="${labor}"></td>
                    <td><input type="number" class="hoursInput hours form-control" data-name="Hours" value="${hours}"></td>
                    <td><input type="number" class="rateInput cost form-control" data-name="Rate" value="${rate}" step="0.01"></td>
                    <td class="total"><span class="cost">${row.children('td').eq(3).text()}</span></td>
                    <td>
                        <span class="icon save-labor"><i class="icon-floppy-disk"></i></span>
                        <span class="icon delete-labor"><i class="icon-trash"></i></span>
                    </td>
                `);
        refreshLaborIndexes();
        updateTotalHours();
        updateTotalCost();
    });

    $(document).off('click', '.delete-labor');
    $(document).on('click', '.delete-labor', function () {
        $(this).closest('tr').remove();
        refreshLaborIndexes();
        updateTotalHours();
        updateTotalCost();
    });

    refreshLaborIndexes();
    updateTotalHours();
    updateTotalCost();
});

function refreshLaborIndexes() {
    $('#laborTable tbody tr').each(function (index, row) {
        $(row).find('input').each(function () {
            const name = $(this).data('name');
            $(this).attr('name', `TaskLabors[${index}].${name}`);
        });
    });
}

function hasUnsavedLaborRows() {
    let hasUnsaved = false;
    $('#laborTable tbody tr').each(function () {
        if ($(this).find('input:visible').length > 0) {
            hasUnsaved = true;
        }
    });
    return hasUnsaved;
}

function CheckLaborValidation(laborInput, hoursInput, rateInput) {
    let isValid = true;
    if (laborInput.val().trim() === '') {
        isValid = false;
        laborInput.css('border-color', 'red');
    } else {
        laborInput.css('border-color', '');
    }

    if (hoursInput.val().trim() === '') {
        isValid = false;
        hoursInput.css('border-color', 'red');
    } else {
        hoursInput.css('border-color', '');
    }

    if (rateInput.val().trim() === '') {
        isValid = false;
        rateInput.css('border-color', 'red');
    } else {
        rateInput.css('border-color', '');
    }
    return isValid;
}
function updateTotalHours() {
    let totalHours = 0;
    $('.hours').each(function () {
        totalHours += parseFloat($(this).val()) || 0;
    });
    $('#budget-hours').html(totalHours);
    $('#budget-hours-input').val(totalHours);
}