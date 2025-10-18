$(document).ready(function () {
    $('.unsaved-step-error').hide();
    function refreshStepIndexes() {
        $('#stepTable tbody tr').each(function (index, row) {
            $(row).find('input').each(function () {
                const name = $(this).data('name');
                $(this).attr('name', `TaskWorkSteps[${index}].${name}`);
            });
        });
    }

    function hasUnsavedStepRows() {
        let hasUnsaved = false;
        $('#stepTable tbody tr').each(function () {
            if ($(this).find('input:visible').length > 0) {
                hasUnsaved = true;
            }
        });
        return hasUnsaved;
    }


    $('#addStep').click(function () {


        if (hasUnsavedStepRows()) {
            $('.unsaved-step-error').show();
            return;
        }

        $('.unsaved-step-error').hide();
        const newRow = `
                        <tr>
                                    <td><input type="text" class="stepInput form-control" data-name="Title"></td>
                                <td><input type="number" class="orderInput form-control" data-name="Order"></td>
                            <td>
                                <span class="icon save-step"><i class="icon-floppy-disk"></i></span>
                                <span class="icon delete-step"><i class="icon-trash"></i></span>
                            </td>
                        </tr>
                    `;
        $('#stepTable tbody').append(newRow);
        refreshStepIndexes();
    });

    $(document).off('click', '.save-step');
    $(document).on('click', '.save-step', function () {
        const row = $(this).closest('tr');
        const stepInput = row.find('.stepInput');
        const orderInput = row.find('.orderInput');
        const errorDiv = row.find('.error');

        let isValid = CheckStepValidation();
      
        if (isValid) {
            const step = stepInput.val();
            const order = parseFloat(orderInput.val());

            row.html(`
                            <td>
                                ${step}
                                    <input type="hidden" name="${stepInput.attr('name')}" value="${step}"  data-name="Title">
                            </td>
                            <td>
                                ${order}
                                    <input type="hidden" name="${orderInput.attr('name')}" value="${order}" data-name="Order">
                            </td>
                            
                            <td>
                                <span class="icon edit-step"><i class="icon-pencil5"></i></span>
                                <span class="icon delete-step"><i class="icon-trash"></i></span>
                            </td>
                        `);
            refreshStepIndexes();
        } else {
            errorDiv.show();
        }

        function CheckStepValidation() {
            let isValid = true;
            if (stepInput.val().trim() === '') {
                isValid = false;
                stepInput.css('border-color', 'red');
            } else {
                stepInput.css('border-color', '');
            }

            if (orderInput.val().trim() === '') {
                isValid = false;
                orderInput.css('border-color', 'red');
            } else {
                orderInput.css('border-color', '');
            }
            return isValid;
        }
    });

    $(document).off('click', '.edit-step');
    $(document).on('click', '.edit-step', function () {
        const row = $(this).closest('tr');
        const step = row.children('td').eq(0).text().trim();
        const order = row.children('td').eq(1).text().trim();

        row.html(`
                            <td><input type="text" class="stepInput form-control" data-name="Title" value="${step}"></td>
                            <td><input type="number" class="orderInput form-control" data-name="Order" value="${order}"></td>
                        <td>
                            <span class="icon save-step"><i class="icon-floppy-disk"></i></span>
                            <span class="icon delete-step"><i class="icon-trash"></i></span>
                        </td>
                    `);
        refreshStepIndexes();
    });

    $(document).off('click', '.delete-step');
    $(document).on('click', '.delete-step', function () {
        $(this).closest('tr').remove();
        refreshStepIndexes();
    });

    refreshStepIndexes();
});