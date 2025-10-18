$(function () {
    // Unbind any existing double-click event handler on elements with the class .td-html-span
    $("body").off("dblclick", ".td-html-span");

    // Bind a double-click event handler to elements with the class .td-html-span
    $("body").on("dblclick", ".td-html-span", handleCellEdit);

    // Unbind and rebind change events for input and select elements within .td-html-span
    $("body").off("change", ".td-html-span>input,.td-html-span>select");
    $("body").on("change", ".td-html-span>input,.td-html-span>select", function () {
        validateCellContent(this);
    });
    // Unbind and rebind click events for cancel and save buttons
    $("body").off("click", ".edit-cell-cancel-btn");
    $("body").on("click", ".edit-cell-cancel-btn", handleCancelClick);

    $("body").off("click", ".edit-cell-save-btn");
    $("body").on("click", ".edit-cell-save-btn", handleSaveClick);
});

function handleCellEdit(e) {
    var element = e.currentTarget;

    if (!$(element).hasClass("processing")) {
        showLoader();

        // Hide any visible tooltips
        $('[data-bs-toggle="tooltip"]').tooltip('hide');

        var name = $(element).attr("attr-cell-name");
        var fieldName = $(element).attr("attr-field-name");
        $(element).attr("attr-cell-html", $(element).html().trim());

        var formId = $(element).attr("attr-entity-form-id");
        var additionalFields = JSON.parse($(element).attr("attr-additional-fields"));
        var formElement = $(`#${formId}`).find(`[name='${fieldName}']`);
        var html = formElement.closest(".form-group").html();

        var elementContainer = $(element);

        var currentValue;
        if ($(element).attr("attr-entity-is-tooltip") === "true") {
            currentValue = $(element).find('.tooltip-wide').text().trim();
        } else {
            currentValue = $(elementContainer).html().trim();
        }
        var actualValue = $(element).attr("attr-cell-data");

        $(elementContainer).html(addFormButtons(html));
        $(elementContainer).find("label.form-label").remove();
        $(elementContainer).closest("td").css("width", "200px");

        initializeInputFields(elementContainer, additionalFields, currentValue);

        $(element).addClass("processing");
        hideLoader();
        updateColumnSetting();
    }
}

function initializeInputFields(elementContainer, additionalFields, currentValue) {
    if ($(elementContainer).find(".customized-select").length > 0) {
        setupSelect2(elementContainer, additionalFields);
    }

    if ($(elementContainer).find("select").length > 0) {
        selectCurrentOption(elementContainer, currentValue);
    } else if ($(elementContainer).find("input:radio").length > 0) {
        selectCurrentRadio(elementContainer, currentValue);
    } else {
        $(elementContainer).find("input").val(currentValue);
    }
}

function setupSelect2(elementContainer, additionalFields) {
    let newSelect2Id = uid();
    var select2 = $(elementContainer).find(".customized-select");
    select2.attr("id", newSelect2Id);
    let select2FunctionName = select2.data("select2-initialize-function");
    let selectedId = additionalFields[0].Select2SelectedId;
    let selectedValue = additionalFields[1].Select2SelectedValue;
    window[select2FunctionName]("#" + newSelect2Id);
    var newOption = new Option(selectedValue, selectedId, true, true);
    $("#" + newSelect2Id).append(newOption).trigger('change');
}

function selectCurrentOption(elementContainer, currentValue) {
    $(elementContainer).find("select").find('option').each(function (i, v) {
        if ($(v).text() === currentValue) {
            $(v).prop('selected', true);
        }
    });
}

function selectCurrentRadio(elementContainer, currentValue) {
    $(elementContainer).find("input:radio").each(function (i, radio) {
        var label = $(radio).siblings("label").html();
        if (label === currentValue) {
            $(radio).prop('checked', true);
            return false;
        }
    });
}

function handleCancelClick(e) {
    e.stopPropagation();
    var td = $(this).closest(".td-html-span");
    updateCellContent(td);
}

function handleSaveClick(e) {
    e.stopPropagation();
    var td = $(this).closest("td");
    var input = td.find("input, select");
    if (validateCellContent(input)) {
        saveCellContent(input);
    }
}

function validateCellContent(element) {
    var clonedElement = cloneElementWithSelectedOption(element);
    updateFormWithClonedElementInputGroup(clonedElement, element);

    if (!isFormValid()) {
        updateCellWithValidationError(element);
        return false;
    }
    return true;
}

function cloneElementWithSelectedOption(element) {
    let clonedElement = $(element).clone();
    setSelectOptionForClone(element, clonedElement);
    return clonedElement;
}

function updateFormWithClonedElementInputGroup(clonedElement, originalElement) {
    let validationSpan = $(originalElement).closest(".td-html-span").find('.validation-invalid-label').prop('outerHTML');
    $("#data-cell-edit-form").empty().html(clonedElement).append(validationSpan);
}

function isFormValid() {
    let form = $("#data-cell-edit-form");
    form.removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);
    return form.valid();
}

function updateCellWithValidationError(element) {
    var clonedForm = $("#data-cell-edit-form").html();
    $(element).closest(".td-html-span").html(addFormButtons(clonedForm));
}
function setSelectOptionForClone(element, clone) {
    if ($(element).is('select')) {
        var value = $(element).find("option:selected").text();
        $(clone).find('option').each(function (i, v) {
            if ($(v).text() === value) {
                $(v).prop('selected', true);
            }
        });
    }
}
function saveCellContent(element) {
    showLoader();

    var $element = $(element).closest(".td-html-span");
    var { id, propertyName, originalValue } = getElementAttributes($element);

    var { propertyValue, propertyText } = getElementValueAndText(element);

    var url = buildSaveUrl($element, propertyValue);

    $.ajax({
        url: url,
        type: "get",
        dataType: "html",
        success: function (response) {
            handleSaveSuccess(response, $element, element, propertyText, propertyValue);
        },
        error: handleSaveError,
    });
}

function getElementAttributes($element) {
    return {
        id: $element.attr("attr-entity-id"),
        propertyName: $element.attr("attr-cell-name"),
        originalValue: $element.attr("attr-cell-html"),
    };
}

function getElementValueAndText(element) {
    let propertyValue = $(element).val();
    let propertyText = propertyValue;
    let isElementSelect2 = $(element).hasClass('customized-select');
    let isElementSelect = $(element).is('select');
    let isElementRadio = $(element).is('input:radio');

    if (isElementSelect2) {
        let elementId = $(element).attr('id');
        propertyText = $("#" + elementId).select2('data')[0]?.text;
    }
    else if (isElementSelect) {
        propertyValue = $(element).find('option:selected').val();
        propertyText = $(element).find('option:selected').text();
    }
    else if (isElementRadio) {
        propertyValue = $(element).filter(":checked").val();
        propertyText = $(element).filter(":checked").closest(".form-check").find("label").html();
    }
    return { propertyValue, propertyText };
}

function buildSaveUrl($element, propertyValue) {
    var entityId = $element.attr("attr-entity-id");
    var entityName = $element.attr("attr-entity-name");
    var entityProperty = $element.attr("attr-entity-property");
    return `/Home/SaveDataTableCell?propertyName=${entityProperty}&propertyValue=${propertyValue}&entityId=${entityId}&entityName=${entityName}`;
}

function handleSaveSuccess(response, $element, element, propertyText, propertyValue) {
    console.log(response);
    if (response) {
        var isElementSelectOrRadio = $(element).is('select') || $(element).is('input:radio');
        updateCellContent($element, isElementSelectOrRadio ? propertyText : propertyValue);
    }
    hideLoader();
}

function handleSaveError(jqXHR, textStatus, errorThrown) {
    console.log(textStatus, errorThrown);
    hideLoader();
}

function updateCellContent(element, updatedValue = null) {
    var $element = $(element);
    $element.removeClass("processing");

    if (updatedValue == null) {
        restoreOriginalContent($element);
    } else {
        updateContent($element, updatedValue);
    }

    $element.closest("td").removeClass("editing");
    updateColumnSetting();
}

function restoreOriginalContent($element) {
    $element.html($element.attr("attr-cell-html"));
}

function updateContent($element, updatedValue) {
    if ($element.attr("attr-entity-is-tooltip") === "true") {
        updateTooltipContent($element, updatedValue);
    } else if ($element.attr("attr-cell-html").includes("badge")) {
        updateBadgeContent($element, updatedValue);
    } else if ($element.find(".customized-select").length > 0) {
        updateSelect2Content($element, updatedValue);
    } else {
        $element.html(updatedValue);
    }

    $element.attr("attr-cell-data", updatedValue);
}

function updateTooltipContent($element, updatedValue) {
    $element.html(addTooltip(updatedValue));
    $('[data-bs-toggle="tooltip"]').tooltip();
}

function updateBadgeContent($element, updatedValue) {
    let previousHtml = $element.attr("attr-cell-html");
    let sanitizedPreviousHtml = $element.attr("attr-cell-data").replace(/\s+/g, '');
    let sanitizedUpdatedValue = updatedValue.replace(/\s+/g, '');
    let updatedHtml = previousHtml.replace(sanitizedPreviousHtml, sanitizedUpdatedValue);
    $element.html(updatedHtml);
}

function updateSelect2Content($element, updatedValue) {
    let select2Element = $element.find(".customized-select")[0];
    let additionalFields = JSON.parse($element.attr("attr-additional-fields"));
    updateAdditionalFieldByKey(additionalFields, 'Select2SelectedId', $(select2Element).val());
    updateAdditionalFieldByKey(additionalFields, 'Select2SelectedValue', $(select2Element).select2('data')[0]?.text);
    $element.attr("attr-additional-fields", JSON.stringify(additionalFields));
    $element.html(updatedValue);
}
function addFormButtons(html) {
    return `${html}<div><button class='btn btn-danger edit-cell-cancel-btn mt-1 mr-1'>Cancel</button><button class='btn btn-success edit-cell-save-btn mt-1 ms-1'>Save</button></div>`;
}

const uid = function () {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
}

function addTooltip(data) {
    const hasTooltip = data?.length > 30;
    const tooltipContent = `data-bs-toggle="tooltip" data-bs-placement="top" title="${encodeHTML(data)}"`;
    return `<span class="tooltip-wide ${encodeHTML(data)}" ${tooltipContent}>${(encodeHTML(data)?.slice(0, 30)) ?? "-"}${hasTooltip ? "..." : ""}</span>`;
}

function encodeHTML(html) {
    if (html == null)
        return "";
    return html?.replace(/&/g, '&amp;')
        .replace(/ /g, '&nbsp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/\//g, '&#47;')
        .replace(/\\/g, '&#92;')
        .replace(/\|/g, '&#124;')
        .replace(/=/g, '&#61;');
}

function updateAdditionalFieldByKey(additionalFields, key, newValue) {
    for (const field of additionalFields) {
        if (field[key]) {
            field[key] = newValue;
            break; // Stop searching after finding the first match
        }
    }
}

function updateColumnSetting() {
    // Placeholder function for updating column settings
}