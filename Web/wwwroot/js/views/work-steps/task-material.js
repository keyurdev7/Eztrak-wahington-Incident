$(document).ready(function () {
    $('.unsaved-material-error').hide();
    function refreshMaterialIndexes() {
        $('#materialTable tbody tr').each(function (index, row) {
            $(row).find('input').each(function () {
                const name = $(this).data('name');
                $(this).attr('name', `TaskMaterials[${index}].${name}`);
            });
        });
    }

    function hasUnsavedMaterialRows() {
        let hasUnsaved = false;
        $('#materialTable tbody tr').each(function () {
            if ($(this).find('input:visible').length > 0) {
                hasUnsaved = true;
            }
        });
        return hasUnsaved;
    }


    $('#addMaterial').click(function () {


        if (hasUnsavedMaterialRows()) {
            $('.unsaved-material-error').show();
            return;
        }

        $('.unsaved-material-error').hide();
        const newRow = `
                        <tr>
                                    <td><input type="text" class="materialInput form-control" data-name="Title"></td>
                                <td><input type="number" class="costInput cost form-control" data-name="Cost"></td>
                            <td>
                                <span class="icon save-material"><i class="icon-floppy-disk"></i></span>
                                <span class="icon delete-material"><i class="icon-trash"></i></span>
                            </td>
                        </tr>
                    `;
        $('#materialTable tbody').append(newRow);
        refreshMaterialIndexes();
    });

    $(document).off('click', '.save-material');
    $(document).on('click', '.save-material', function () {
        const row = $(this).closest('tr');
        const materialInput = row.find('.materialInput');
        const costInput = row.find('.costInput');
        const errorDiv = row.find('.error');

        let isValid = CheckMaterialValidation();
      
        if (isValid) {
            const material = materialInput.val();
            const cost = parseFloat(costInput.val());

            row.html(`
                            <td>
                                ${material}
                                    <input type="hidden" name="${materialInput.attr('name')}" value="${material}"  data-name="Title">
                            </td>
                            <td>
                                $${cost}
                                    <input type="hidden" name="${costInput.attr('name')}" class="cost" value="${cost}" data-name="Cost">
                            </td>
                            
                            <td>
                                <span class="icon edit-material"><i class="icon-pencil5"></i></span>
                                <span class="icon delete-material"><i class="icon-trash"></i></span>
                            </td>
                        `);
            refreshMaterialIndexes();
            updateTotalCost();
        } else {
            errorDiv.show();
        }

        function CheckMaterialValidation() {
            let isValid = true;
            if (materialInput.val().trim() === '') {
                isValid = false;
                materialInput.css('border-color', 'red');
            } else {
                materialInput.css('border-color', '');
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

    $(document).off('click', '.edit-material');
    $(document).on('click', '.edit-material', function () {
        const row = $(this).closest('tr');
        const material = row.children('td').eq(0).text().trim();
        const cost = row.children('td').eq(1).text().replace('$', '').trim();


        row.html(`
                            <td><input type="text" class="materialInput form-control" data-name="Title" value="${material}"></td>
                            <td><input type="number" class="costInput cost form-control" data-name="Cost" value="${cost}"></td>
                        <td>
                            <span class="icon save-material"><i class="icon-floppy-disk"></i></span>
                            <span class="icon delete-material"><i class="icon-trash"></i></span>
                        </td>
                    `);
        refreshMaterialIndexes();
        updateTotalCost();
    });

    $(document).off('click', '.delete-material');
    $(document).on('click', '.delete-material', function () {
        $(this).closest('tr').remove();
        refreshMaterialIndexes();
        updateTotalCost();
    });

    refreshMaterialIndexes();
    updateTotalCost();
});