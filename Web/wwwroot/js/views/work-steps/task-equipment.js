$(document).ready(function () {
    $('.unsaved-equipment-error').hide();
    function refreshEquipmentIndexes() {
        $('#equipmentTable tbody tr').each(function (index, row) {
            $(row).find('input').each(function () {
                const name = $(this).data('name');
                $(this).attr('name', `TaskEquipments[${index}].${name}`);
            });
        });
    }

    function hasUnsavedEquipmentRows() {
        let hasUnsaved = false;
        $('#equipmentTable tbody tr').each(function () {
            if ($(this).find('input:visible').length > 0) {
                hasUnsaved = true;
            }
        });
        return hasUnsaved;
    }


    $('#addEquipment').click(function () {


        if (hasUnsavedEquipmentRows()) {
            $('.unsaved-equipment-error').show();
            return;
        }

        $('.unsaved-equipment-error').hide();
        const newRow = `
                        <tr>
                                    <td><input type="text" class="equipmentInput form-control" data-name="Title"></td>
                                <td><input type="number" class="costInput cost form-control" data-name="Cost"></td>
                            <td>
                                <span class="icon save-equipment"><i class="icon-floppy-disk"></i></span>
                                <span class="icon delete-equipment"><i class="icon-trash"></i></span>
                            </td>
                        </tr>
                    `;
        $('#equipmentTable tbody').append(newRow);
        refreshEquipmentIndexes();
    });

    $(document).off('click', '.save-equipment');
    $(document).on('click', '.save-equipment', function () {
        const row = $(this).closest('tr');
        const equipmentInput = row.find('.equipmentInput');
        const costInput = row.find('.costInput');
        const errorDiv = row.find('.error');

        let isValid = CheckEquipmentValidation();

        if (isValid) {
            const equipment = equipmentInput.val();
            const cost = parseFloat(costInput.val());

            row.html(`
                            <td>
                                ${equipment}
                                    <input type="hidden" name="${equipmentInput.attr('name')}" value="${equipment}"  data-name="Title">
                            </td>
                            <td>
                                $${cost}
                                    <input type="hidden" name="${costInput.attr('name')}" class="cost" value="${cost}" data-name="Cost">
                            </td>
                            
                            <td>
                                <span class="icon edit-equipment"><i class="icon-pencil5"></i></span>
                                <span class="icon delete-equipment"><i class="icon-trash"></i></span>
                            </td>
                        `);
            refreshEquipmentIndexes();
            updateTotalCost();
        } else {
            errorDiv.show();
        }

        function CheckEquipmentValidation() {
            let isValid = true;
            if (equipmentInput.val().trim() === '') {
                isValid = false;
                equipmentInput.css('border-color', 'red');
            } else {
                equipmentInput.css('border-color', '');
            }

            if (costInput.val().trim() === '') {
                isValid = false;
                costInput.css('border-color', 'red');
            } else {
                costInput.css('border-color', '');
            }
            return isValid;
        }
    });

    $(document).off('click', '.edit-equipment');
    $(document).on('click', '.edit-equipment', function () {
        const row = $(this).closest('tr');
        const equipment = row.children('td').eq(0).text().trim();
        const cost = row.children('td').eq(1).text().replace('$', '').trim();

        row.html(`
                            <td><input type="text" class="equipmentInput form-control" data-name="Title" value="${equipment}"></td>
                            <td><input type="number" class="costInput form-control cost" data-name="Cost" value="${cost}"></td>
                        <td>
                            <span class="icon save-equipment"><i class="icon-floppy-disk"></i></span>
                            <span class="icon delete-equipment"><i class="icon-trash"></i></span>
                        </td>
                    `);
        refreshEquipmentIndexes();
        updateTotalCost();
    });

    $(document).off('click', '.delete-equipment');
    $(document).on('click', '.delete-equipment', function () {
        $(this).closest('tr').remove();
        refreshEquipmentIndexes();
        updateTotalCost();
    });

    refreshEquipmentIndexes();
    updateTotalCost();
});