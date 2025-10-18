class DataTableManager {
    constructor() {
        this.dataTable = null;
        this.tableId = null;
        this.formId = null;
        this.controllerName = null;
        this.actionsList = [];
        this.dtColumns = null;
        this.isResponsive = false;
        this.selectableRow = false;
        this.dataAjaxUrl = null;
        this.showSelectedFilters = true;
        this.isAjaxBasedCrud = true;
        this.enableButtons = true;
        this.showDatatableButtons = false;
        this.isExcelDownloadAjaxBased = false;
        this.isScrollableFixedHeaderEventAdded = false;
        this.isPasscodeRequiredForDelete = this.isPasscodeRequiredForDelete;
        this.pageLength = 25;
        this.actionIcons = {
            "Update": "icon-pencil5",
            "Add": "fas fa-circle-plus",
            "Profile": "fas fa-user-plus",
            "Notes": "fas fa-file",
            "View": "fas fa-eye",
            "History": "fas fa-history",
            "Detail": "icon-folder",
            "Approve": "icon-checkmark4",
            "Delete": "icon-trash",
            "Report": "icon-copy",
            "ResetPassword": "icon-key",
            "ResetAccessCode": "icon-lock",
            "Comments": "fas fa-comments",
            "Invoice": "fas fa-file-invoice-dollar",
            "Print": "fas fa-print",
            "Permission": "fas fa-shield-alt",
            "Contact": "fa-solid fa-address-book",
            "Email": "fa-solid fa-paper-plane",
            "Issue": "fas fa-shopping-cart",
            "UpdateStatus": "fas fa-sync",
            "Print": "fa-solid fa-print",
            "Timesheet": "fa-solid fa-clock"
        };
    }

    // Main function to initialize DataTables
    initialize(tableIdParam, formIdParam, dtColumnsParam, showDatatableButtonsParam = false, dataUrl = "", isPasscodeRequiredForDeleteParam = false, controllerNameParam = "", enableButtonsParam = true, isAjaxBasedCrudParam = true, isResponsiveParam = false, selectableRowParam = false, isExcelDownloadAjaxBasedParam = false, pageLength = 25) {

        // console.log("🚀 ~ file: auto-datatable.js:84 ~ DataTableManager ~ initialize(tableIdParam,formIdParam,dtColumnsParam,showDatatableButtonsParam ~ tableIdParam:", tableIdParam);
        // Set instance variables
        this.pageLength = pageLength;
        this.isAjaxBasedCrud = isAjaxBasedCrudParam;
        this.enableButtons = enableButtonsParam;
        this.isExcelDownloadAjaxBased = isExcelDownloadAjaxBasedParam;

        let currentController = window.location.pathname.split('/')[1];
        this.dataAjaxUrl = dataUrl || `/${currentController}/Search`;
        this.tableId = tableIdParam;
        this.formId = formIdParam;// $(".search-form").attr('id');
        this.actionsList = [];
        this.dtColumns = dtColumnsParam;
        this.isResponsive = isResponsiveParam;
        this.selectableRow = selectableRowParam;
        this.showDatatableButtons = showDatatableButtonsParam.toLowerCase();
        this.isPasscodeRequiredForDelete = isPasscodeRequiredForDeleteParam;
        if (controllerNameParam == "" || controllerNameParam == null) {
            this.controllerName = window.location.href.split(/[/#]/)[3];
        }
        else {
            this.controllerName = controllerNameParam;
        }

        showLoader($("#" + this.tableId));

        // Add scroll event listener for fixed header
        this.addScrollEventListener();

        // Add event listeners
        this.addEventListeners();

        // Initialize DataTable
        this.setupDataTable();
    }

    // Add scroll event listener to handle fixed header
    addScrollEventListener() {
        $(window).off('scroll');
        $(window).on('scroll', () => {
            let isFixedHeaderVisible = $('.dtfh-floatingparent.dtfh-floatingparenthead').is(':visible');
            if (isFixedHeaderVisible && !this.isScrollableFixedHeaderEventAdded) {
                this.addScrollableFixedHeaderEvent();
            } else {
                this.isScrollableFixedHeaderEventAdded = false;
            }
        });
    }

    // Add the scrollable fixed header event
    addScrollableFixedHeaderEvent() {
        $('.dtfh-floatingparent.dtfh-floatingparenthead').css({
            'overflow-x': 'scroll'
        }).on('scroll', function () {
            let scrollBody = $(this).parent().find('.dataTables_scrollBody').get(0);
            scrollBody.scrollLeft = this.scrollLeft;
            $(scrollBody).trigger('scroll');
        });
        this.isScrollableFixedHeaderEventAdded = true;
    }

    // Add various event listeners for actions and interactions
    addEventListeners() {
        // Clear form button
        $(document).off('click', '.clear-form-btn');
        $(document).on('click', '.clear-form-btn', () => {
            this.clearDatatableSearch();
        });

        // Enter key in search fields
        $(document).off('keypress', '#filter-form input[type=search],#filter-form input[type=text]');
        $(document).on('keypress', '#filter-form input[type=search],#filter-form input[type=text]', (event) => {
            if (event.keyCode === 13 || event.which === 13) {
                event.preventDefault();
                this.searchDataTable();
            }
        });

        // Clear filter badge
        $(document).off('click', '.badge-datatable-clear');
        $(document).on('click', '.badge-datatable-clear', () => {
            this.clearDatatableSearch();
        });

        // Search filter badge
        $(document).off('click', '.badge-datatable-search');
        $(document).on('click', '.badge-datatable-search', (event) => {
            let name = $(event.target).attr('data-input-name');
            this.#removeSearchFilterInput(name);
        });

        // Search button
        $(document).off('click', '.search-form-btn');
        $(document).on('click', '.search-form-btn', () => {
            this.searchDataTable();
            setTimeout(function () {
                $('.field-filter-container').hide(100);
            }, 200);
        });

        // Show filters
        $(document).off('click', '#search-filters');
        $(document).on('click', '#search-filters', () => {
            $(".search-filter-container").show();
            $(".list-container").hide();
        });

        // Back to list
        $(document).off('click', '#back-to-list');
        $(document).on('click', '#back-to-list', () => {
            $(".list-container").show();
        });

        // Delete action
        $(document).off('click', '.delete');
        $(document).on('click', '.delete', (e) => {
            e.preventDefault();
            let deleteObj = {
                deleteUrl: '',
                confirmBtnText: "",
                cancelBtnText: "",
                deleteReturnUrl: ""
            };
            // Check if the clicked element or its ancestor is an <a> element
            let $deleteLink = $(e.target).closest('a');
            if ($deleteLink.length > 0) {
                deleteObj.deleteUrl = $deleteLink.attr('href');
            }
            if (this.isPasscodeRequiredForDelete == "True") {
                DeleteItem(deleteObj, this);
            } else {
                DeleteDataItem(deleteObj, this);
            }
        });


        // Expand row action
        $(document).off('click', '.dt-control.expand');
        $(document).on('click', '.dt-control.expand', (e) => {
            let tr = $(e.target).closest('tr');
            let row = this.dataTable.row(tr);

            if (row.child.isShown()) {
                row.child.hide();
                tr.removeClass('dt-hasChild shown');
            } else {
                showLoader(`#${this.tableId}`);
                let rowData = row.data();
                let url = `/${this.controllerName}/GetExpandedData/${rowData.Id}`;

                $.ajax({
                    url: url,
                    method: 'GET',
                    success: (response) => {
                        row.child(response).show();
                        tr.addClass("dt-hasChild shown");
                        hideLoader(`#${this.tableId}`);
                    },
                    error: (xhr, status, error) => {
                        hideLoader(`#${this.tableId}`);
                        console.error('AJAX Error:', error);
                    }
                });
            }
        });
    }

    // Setup DataTable with configuration
    setupDataTable() {
        let selectableRowObj = this.selectableRow ? {
            orderable: true,
            className: 'select-checkbox',
            targets: 0,
            data: null,
            defaultContent: ''
        } : {};

        let columnsIndexExcludingAction = [];
        for (var i = 0; i <= (this.dtColumns.length - 1); i++) {
            columnsIndexExcludingAction.push(i);
        }
        // Add toggle filters button to the header
        let toggleFiltersButton = $(`
            <button class="btn btn-sm btn-outline-secondary toggle-filters ms-2">
                <i class="fas fa-filter"></i> Show Filters
            </button>
        `);
        // Create filter row but initially hide it
        const that = this;

        let datatableButtonClass = this.showDatatableButtons == "true" ? "" : "hide-datatable-buttons";
        this.dataTable = $('#' + this.tableId).DataTable({
            "serverSide": true,
            "proccessing": true,
            "searching": true,
            "autoWidth": true,
            "fixedHeader": true,
            orderCellsTop: true,
            scrollX: true,
            "pageLength": this.pageLength,
            "responsive": this.isResponsive,
            "ordering": false,
            "order": [],//#getColumnSortings(dtColumns),
            "pagingType": "full_numbers",
            "lengthMenu": [[10, 25, 50, 250, 1000, 5000, -1], [10, 25, 50, 250, 1000, 5000, "All"]],
            //lBfrtipF
            "dom": "<'datatable-header mb-2 d-flex flex-wrap justify-content-between'" +
                "<'datatable-button-container " + datatableButtonClass + "'B>" +
                "<'datatable-fp-container'l" +
                "<'toggle-filters-container ms-2'>" + //"<'ms-2'f>"
                ">" +
                ">" +
                "<'datatable-scroll'tr>" +
                "<'datatable-footer'<'custom-footer-row'> ip'l>",
            "language": {
                "info": "Showing _START_ - _END_ of _TOTAL_ items", // Update the info text
                "infoEmpty": "No records available",
                "infoFiltered": "(filtered from _MAX_ total records)",
                "search": '<span class="me-2">Filter:</span>',
                "searchPlaceholder": "Search",
                "lengthMenu": '<span class="me-2"> </span> _MENU_', // Keep the length menu as is
                'paginate': {
                    'previous': `<a href="#">
						<svg width="5" height="8" viewBox="0 0 5 8" fill="none" xmlns="http://www.w3.org/2000/svg">
							<path d="M4.42871 1.13086L1.58138 3.9782L4.42868 6.82556" stroke="#252432" stroke-linecap="round"></path>
						</svg>
					</a>`,
                    'next': `<a href="#">
						<svg width="5" height="8" viewBox="0 0 5 8" fill="none" xmlns="http://www.w3.org/2000/svg">
							<path d="M1.42188 1.13086L4.2692 3.9782L1.4219 6.82556" stroke="#252432" stroke-linecap="round"></path>
						</svg>
					</a>`,
                    'first': `<a href="#">
						<svg width="9" height="8" viewBox="0 0 9 8" fill="none" xmlns="http://www.w3.org/2000/svg">
							<path d="M7.87305 1.13086L5.02572 3.9782L7.87302 6.82556" stroke="#252432" stroke-linecap="round"></path>
							<path d="M3.74902 1.13086L0.901695 3.9782L3.749 6.82556" stroke="#252432" stroke-linecap="round"></path>
						</svg>
					</a>`,
                    'last': `<a href="#">
						<svg width="9" height="8" viewBox="0 0 9 8" fill="none" xmlns="http://www.w3.org/2000/svg">
							<path d="M0.977051 1.13086L3.82438 3.9782L0.977078 6.82556" stroke="#252432" stroke-linecap="round"></path>
							<path d="M5.10156 1.13086L7.94889 3.9782L5.10159 6.82556" stroke="#252432" stroke-linecap="round"></path>
						</svg>
					</a>`
                }
            },
            "ajax": {
                url: this.dataAjaxUrl,
                type: 'POST',
                dataType: "json",
                "data": (searchParams) => {
                    $("#" + this.tableId + " td").removeAttr("colspan");
                    if ($("#loader").length > 0) {
                        $('#' + this.tableId).css("min-height", "200px");
                        $('#' + this.tableId).block({
                            message: $("#loader"),
                            centerY: false,
                            centerX: false,
                            css: {
                                margin: 'auto',
                                border: 'none',
                                padding: '15px',
                                backgroundColor: 'transparent',
                                '-webkit-border-radius': '10px',
                                '-moz-border-radius': '10px',
                                color: '#fff'
                            }

                        });
                    }
                    // console.log("🚀 ~ file: auto-datatable.js:319 ~ DataTableManager ~ setupDataTable ~ this.formId:", this.formId, that.formId);
                    // debugger;
                    if ($('#' + this.formId).length > 0) {
                        $('#' + this.formId + ' :input, #' + this.formId + ' select').each(function (key, val) {
                            if (val.value !== "" && !(val.name in searchParams)) { // Check if the element has a non-empty value and isn't already in searchParams
                                if ($(val).is(":checkbox")) { // Check if the element is a checkbox
                                    searchParams[val.name] = $(val).is(":checked"); // Store its checked status
                                } else {
                                    searchParams[val.name] = $(val).val(); // Store its value
                                }
                            }
                        });
                    }
                    if (searchParams.length === -1) {
                        searchParams["DisablePagination"] = true;
                    }
                    else {
                        searchParams["DisablePagination"] = false;
                    }

                    searchParams["CurrentPage"] = (searchParams.start / searchParams.length) + 1;
                    searchParams["PerPage"] = searchParams.length;
                    searchParams["CalculateTotal"] = true;
                    if (searchParams.order.length > 0) {
                        searchParams["OrderByColumn"] = this.dtColumns[searchParams.order[0].column].sortingColumn;
                        searchParams["OrderDir"] = searchParams.order[0].dir;
                    }
                    searchParams["Draw"] = searchParams.draw;
                    return searchParams;
                },
                "dataSrc": (json) => {
                    this.actionsList = json.ActionsList;
                    this.showSelectedFilters = json.ShowSelectedFilters;
                    //this.#setSearchFilters(showSelectedFilters);
                    this.afterDataCallBack(json);
                    return json.Items;
                }
            },
            columns: this.dtColumns,
            "rowCallback": function (row, data) {
                if (data.hasOwnProperty('Id')) {
                    $(row).attr('id', data.Id);
                }
                if (data.hasOwnProperty('Title')) {
                    $(row).attr('title', data.Title);
                }
                if (data.DataTableRowClass !== undefined && data.DataTableRowClass !== null) {
                    $(row).addClass(data.DataTableRowClass);
                }
            },
            "columnDefs": [
                selectableRowObj,
                {
                    "targets": columnsIndexExcludingAction,
                    "render": (data, type, row, meta) => {
                        if (this.dtColumns[meta.col].title !== 'Action') {
                            let cellHtml = "";

                            var column = this.dtColumns[meta.col];
                            switch (column.format) {
                                case 'expand':
                                    cellHtml = '';
                                    break;
                                case 'html':
                                    if (type === 'export' && column.exportColumn != null && column.exportColumn !== "") {
                                        cellHtml = row[column.exportColumn];
                                    } else {
                                        cellHtml = this.#renderHtml(data, this.dtColumns, meta, row);
                                    }
                                    break;
                                case 'numeric':
                                    cellHtml = this.#renderNumericValue(data, this.dtColumns, meta);
                                    break;
                                case 'editable':
                                    cellHtml = `<span>${this.#renderNumericValue(data, this.dtColumns, meta)}</span>`;
                                    break;
                                case 'dictionary':
                                    let keyStr = this.dtColumns[meta.col].data;
                                    let value = getValueFromKeyString(row, keyStr);
                                    cellHtml = value !== undefined ? value : "-"
                                    break;
                                default:
                                    if (column.title === '' || column.data === '') {
                                        cellHtml = '';
                                    }
                                    else if (column.className != null && column.className.indexOf("select-all-checkbox") !== -1) {// Dont show id for select checkbox
                                        cellHtml = '';
                                    }
                                    else {
                                        cellHtml = data ?? "-";
                                    }
                                    break;
                            }
                            if (column.isEditable) {
                                try {
                                    let identifierPath = "row." + column.editableColumnDetail.dataIdentifier;
                                    let rowIdentifier = eval(identifierPath);
                                    var attrData = createEditableColumn(row, data, column, rowIdentifier);
                                    cellHtml = `<div class='td-html-span' ${attrData}>${cellHtml}</div>`;
                                }
                                catch {

                                }
                            }
                            return cellHtml;
                        }
                        else {
                            return this.#getActionLinks(data);
                        }
                    }
                },
                {
                    "targets": -1,
                    "className": "text-right",
                },
            ],
            select: {
                style: 'multi',
                selector: 'td:first-child'
            },
            "buttons": {
                dom: {
                    button: {
                        tag: 'button',
                        className: 'btn rounded-round bg-custom-dark datatable-extension-button'
                    }
                },
                'buttons': [
                    {
                        extend: 'copy',
                        exportOptions: {
                            columns: getColumnsToExport,
                            page: 'current',
                            orthogonal: "export"
                        }
                    },
                    {
                        extend: 'csv',
                        exportOptions: {
                            columns: getColumnsToExport,
                            page: 'current',
                            orthogonal: "export"
                        }
                    },
                    {
                        extend: 'excel',
                        exportOptions: {
                            columns: getColumnsToExport,
                            page: 'current',
                            orthogonal: "export",
                        },
                        action: function (e, dt, button, config) {
                            if (this.isExcelDownloadAjaxBased) {
                                var controllerName = window.location.href.split("/")[3];
                                var excelDataDownloaderUrl = '/' + controllerName + '/DownloadExcel';
                                var selectedIds = [];

                                $('tr.selected').each(function () {
                                    var id = $(this).attr('id');
                                    selectedIds.push(id);
                                });
                                var excelDataFilters = dt.ajax.params();
                                excelDataFilters.SelectedIds = selectedIds;
                                excelDataFilters.DisablePagination = true;
                                $.ajax({
                                    url: excelDataDownloaderUrl,
                                    type: 'POST',
                                    data: excelDataFilters,
                                    xhrFields: {
                                        responseType: 'blob'
                                    },
                                    success: function (data) {
                                        var blob = new Blob([data], {
                                            type: 'application/vnd.ms-excel'
                                        });
                                        var link = document.createElement('a');
                                        link.href = window.URL.createObjectURL(blob);
                                        link.download = controllerName + ' - Torrance.xlsx';
                                        document.body.appendChild(link);
                                        link.click();
                                        document.body.removeChild(link);
                                    },
                                    error: function (xhr, textStatus, errorThrown) {
                                        console.log('Error downloading Excel file: ' + errorThrown);
                                    }
                                });
                            } else {
                                // Call the default behavior
                                $.fn.dataTable.ext.buttons.excelHtml5.action.call(this, e, dt, button, config);
                            }
                        }
                    },
                    //{
                    //    extend: 'pdf',
                    //    exportOptions: {
                    //        columns: getColumnsToExport,
                    //        page: 'current',
                    //        orthogonal: "export"
                    //    },
                    //    customize: customizePdfExport
                    //},
                    //{
                    //    extend: 'print',
                    //    exportOptions: {
                    //        columns: getColumnsToExport,
                    //        page: 'current',
                    //        orthogonal: "export"
                    //    }
                    //},
                    'colvis'
                ]
            },
            initComplete: function (settings, json) {
                const api = this.api();

                // console.log("🚀 ~ file: auto-datatable.js:533 ~ DataTableManager ~ that.dtColumns:", that.dtColumns);
                const filterColumns = that.dtColumns.filter(c => c.hasFilter);
                // console.log("🚀 ~ file: auto-datatable.js:532 ~ DataTableManager ~ setupDataTable ~ filterColumns:", filterColumns);
                $(`.dataTables_scrollHeadInner table thead tr:first-child th`).each(function () {
                    const originalTitle = $(this).text();
                    const columnId = filterColumns.find(c => c.title == originalTitle)?.filterId;
                    // console.table("🚀 ~ file: auto-datatable.js:532 ~ DataTableManager ~ originalTitle:", originalTitle, columnId);
                    // console.log("🚀 ~ file: auto-datatable.js:536 ~ DataTableManager ~ columnId:", columnId);
                    if (originalTitle && columnId) {
                        $(this).html(`
                        <div class="d-flex align-items-center justify-content-center">
                            <span>${originalTitle}</span>
                            <i class="fas fa-filter filter-icon cursor-pointer ms-2" data-filter-id="${columnId}" style="opacity: 0.6;"></i>
                        </div>
                        <div class="filter-container field-filter-container" style="display: none;">
                            <div class="filter-actions m-2 text-end">
                                <button class="btn btn-sm btn-outline-secondary clear-filter mr-1">Clear</button>
                                <button class="btn btn-sm btn-secondary apply-filter">Search</button>
                            </div>
                        </div>
                        `);
                    }
                    // <input type="text" class="form-control form-control-sm column-filter" 
                    //        placeholder="Filter ${originalTitle}">
                });

                // Handle filter icon clicks
                $(`.dataTables_scrollHeadInner table .filter-icon`).on('click', function (e) {
                    e.stopPropagation();

                    const filterContainer = $(this).closest('th').find('.filter-container');
                    const filterId = $(this).data('filter-id');
                    const fieldName = $(`.filter .control_field[data-filter-id="${filterId}"]`).find('input, select, .select2').attr('name');

                    $(`.filter .control_field[data-filter-id="${filterId}"]`).appendTo(filterContainer);

                    if (fieldName != undefined && $(`form.filter input[name="${fieldName}"]`).length == 0) {
                        $(`form.filter`).append(`<input type="hidden" name="${fieldName}" value="">`);
                    }
                    // $(`.filter .control_field[data-filter-id="${filterId}"]`).clone().appendTo(filterContainer);
                    // $(`.dataTable .filter-container`).not(filterContainer).slideUp(200);

                    // Toggle this filter
                    if (filterContainer.is(':hidden')) {
                        $('.field-filter-container').hide(100);
                        filterContainer.show(200);
                    }

                    //// Focus the input if showing
                    //if (filterContainer.is(':hidden')) {
                    //    $(this).css('opacity', '0.6');
                    //} else {
                    //    filterContainer.find('input').focus();
                    //    $(this).css('opacity', '1');
                    //}
                });

                $('.field-filter-container').on('change', 'input, select, .select2', function () {
                    // console.log('Field filter container element changed:', this);
                    // const fieldId = $(this).closest('.control_field').data('field-id');
                    const fieldName = $(this).attr('name');
                    console.log("🚀 ~ file: auto-datatable.js:594 ~ DataTableManager ~ $ ~ fieldName:", fieldName);
                    const value = $(this).val();
                    $(`form.filter input[name="${fieldName}"]`).val(value);
                });

                $('.apply-filter').on('click', function () {

                    //get all form fields
                    $('.field-filter-container').find('input, select, .select2').each(function () {
                        const fieldName = $(this).attr('name');
                        const value = $(this).val();
                        // console.log("🚀 ~ file: auto-datatable.js:617 ~ DataTableManager ~ fieldName:", fieldName, value);
                        if (value != "undefined" && value != "" && fieldName != "undefined") {
                            $(`form.filter input[name="${fieldName}"]`).val(value);
                        }
                    });

                    $('.search-form-btn').click();
                });

                $('.filter-actions .clear-filter').on('click', function () {
                    const fieldName = $(this).closest('.filter-container').find('input, select, .select2').attr('name');
                    const selectField = $(`select[name="${fieldName}"]`);
                    if (selectField.data('select2')) {
                        selectField.val('').trigger('change');
                    } else {
                        $(`form.filter input[name="${fieldName}"]`).val('');
                    }
                    $('.search-form-btn').click();
                });

                // Setup column filters
                api.columns().every(function (index) {
                    const column = this;

                    // Add input event handler with debounce
                    let timeoutId;
                    $(`.dataTables_scrollHeadInner table thead th:eq(${index}) .column-filter`).on('keyup change', function () {
                        clearTimeout(timeoutId);
                        timeoutId = setTimeout(() => {
                            if (column.search() !== this.value) {
                                column.search(this.value).draw();
                            }
                        }, 300);
                    });
                });

                // Close filters when clicking outside
                $(document).on('click', function (e) {
                    if (!$(e.target).closest('.filter-container').length &&
                        !$(e.target).closest('.filter-icon').length) {
                        $(`#${that.tableId} .filter-container`).slideUp(200);
                        $(`#${that.tableId} .filter-icon`).css('opacity', '0.6');
                    }
                });
            },
            "drawCallback": (settings) => {
                $('#' + this.tableId).css("min-height", "auto");
                $('#' + this.tableId).unblock();
                new CallBackFunctionality().GetFunctionality();
                maskDatatableCurrency("td.dt-currency", ('#' + this.tableId));
                maskDatatableTelephone("td.dt-telephone", ('#' + this.tableId));

                // that.afterDataCallBack(settings);

                // Add scroll bar on top
                $('.dataTables_scrollHead').off('scroll');
                $('.dataTables_scrollHead').css({
                    'overflow-x': 'scroll'
                }).on('scroll', function (e) {
                    var scrollLeft = this.scrollLeft;
                    if (!$('.dtfh-floatingparent.dtfh-floatingparenthead').is(':visible')) {
                        var scrollBody = $(this).parent().find('.dataTables_scrollBody').get(0);
                        scrollBody.scrollLeft = scrollLeft;
                        $(scrollBody).trigger('scroll');
                    }
                });
                this.dataTable.columns.adjust();// To fix header and body size in sync after sorting with horizontal scroll
            }
        });
        $('#' + this.tableId).off('length.dt');
        $('#' + this.tableId).on('length.dt', (e, settings, len) => {
            this.pageLength = len;
            // You can perform custom actions here
        });
    }
    afterDataCallBack(json) {

    }
    #getColumnSortings(columns) {
        var sortedColumns = [];
        $.each(columns, function (i, v) {
            if (v.orderable) {
                console.log("orderable column");
                var sortedColumn = [i, 'asc'];
                sortedColumns.push(sortedColumn);
            }
        });
        return sortedColumns;
    }
    #getActionLinks(cellData) {
        var actionHtml = "";
        if (this.actionsList.length > 5) {

            actionHtml = "<div class='list-icons'>";
            actionHtml += "<div class='dropdown show'>";
            actionHtml += "<div class='dropdown show'>";
            actionHtml += "<a href='#' class='list-icons-item' data-toggle='dropdown' aria-expanded='true'>";
            actionHtml += "<i class='icon-menu9'></i>";
            actionHtml += "</a>";
            actionHtml += "<div class='dropdown-menu dropdown-menu-right'>";

            $.each(this.actionsList, (index, actionItem) => {
                if (cellData.IsUserDefined === false || cellData.IsUserDefined === undefined) {
                    let href = this.#getHref(actionItem, cellData);
                    if (href != null && href != undefined && href != "") {
                        actionHtml += href;
                    }
                }
            });
            actionHtml += "</div>";
        }
        else {
            $.each(this.actionsList, (index, actionItem) => {
                //to avoid the system defined record in Account table to be deleted.
                if (cellData.IsUserDefined === false || cellData.IsUserDefined === undefined) {
                    let href = this.#getHref(actionItem, cellData);
                    if (href != null && href != undefined && href != "") {
                        actionHtml += href;
                    }
                }
            });
        }
        return actionHtml;
    }
    #getHref(actionItem, cellData) {
        if (actionItem.HideBasedOn != null && actionItem.HideBasedOn != undefined && actionItem.HideBasedOn != "") {
            if (cellData[actionItem.HideBasedOn])
                return "";
        }

        var link = "javascript:void(0)";
        if (actionItem.Href !== '') {
            const replacedHref = actionItem.Href.replace(/{([^}]+)}/g, (match, param) => {
                return GetCellObjectValue(cellData, param.trim());
            });
            link = replacedHref;
        }
        var appendClass = "";
        var dataAttributes = "";
        if (actionItem.Action === "Delete") {
            appendClass = "delete";
        }
        if (actionItem.Action === "Cancel") {
            appendClass = "cancel";
            dataAttributes = "data-return-url='" + actionItem.ReturnUrl + "'";
        }
        if (actionItem.Class !== null && actionItem.Class !== "") {
            appendClass = " " + actionItem.Class;
        }
        let cssClass = getFinalCellClasses(appendClass, cellData);
        var customAttributes = "";
        if (actionItem.Attr !== null && actionItem.Attr.length > 0) {
            actionItem.Attr.forEach(function (v, i) {
                customAttributes += "attr-" + v.toLowerCase() + '="' + GetCellObjectValue(cellData, v) + '" ';
            });
        }
        if (
            actionItem.DisableBasedOn != null
            && actionItem.DisableBasedOn != undefined
            && actionItem.DisableBasedOn.Property != ""
            && (String(cellData[actionItem.DisableBasedOn.Property]) == actionItem.DisableBasedOn.Value)
        ) {
            cssClass += " disabled";
            return '<a href="#" ' + dataAttributes + ' disabled="disabled" class="datatable-action ' + cssClass + '" ' + customAttributes + '> <i class="' + this.actionIcons[actionItem.Title] + '"></i> ' + actionItem.LinkTitle + '</a > ';
        }
        else {
            let tooltipAttributes = '';
            if (actionItem.Tooltip) { // Check if actionItem.Tooltip has a value
                tooltipAttributes += `data-bs-toggle="tooltip" data-bs-placement="top" data-bs-original-title="${actionItem.Tooltip}"`;
            }
            if (actionItem.DatatableHrefType == "Modal") {
                if (actionItem.Title === 'Add' || actionItem.Title === 'Update' || actionItem.Title === 'Notes' || actionItem.Title === 'History') {
                    const svgIcon = actionItem.Title === 'Add'
                        ? `<svg width="18" height="18" viewBox="0 0 18 1918 fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M8.60449 14.3877H9.60449V10.3877H13.6045V9.3877H9.60449V5.3877H8.60449V9.3877H4.60449V10.3877H8.60449V14.3877ZM9.10774 18.8877C7.86324 18.8877 6.69316 18.6515 5.59749 18.1792C4.50199 17.7069 3.54899 17.0659 2.73849 16.2562C1.92799 15.4465 1.28641 14.4944 0.813742 13.3997C0.340909 12.3052 0.104492 11.1356 0.104492 9.89095C0.104492 8.64645 0.340659 7.47636 0.812992 6.38069C1.28533 5.28519 1.92633 4.3322 2.73599 3.52169C3.54566 2.71119 4.49783 2.06961 5.59249 1.59695C6.68699 1.12411 7.85658 0.887695 9.10124 0.887695C10.3457 0.887695 11.5158 1.12386 12.6115 1.5962C13.707 2.06853 14.66 2.70953 15.4705 3.5192C16.281 4.32886 16.9226 5.28103 17.3952 6.3757C17.8681 7.4702 18.1045 8.63978 18.1045 9.88445C18.1045 11.1289 17.8683 12.299 17.396 13.3947C16.9237 14.4902 16.2827 15.4432 15.473 16.2537C14.6633 17.0642 13.7112 17.7058 12.6165 18.1784C11.522 18.6513 10.3524 18.8877 9.10774 18.8877ZM9.10449 17.8877C11.3378 17.8877 13.2295 17.1127 14.7795 15.5627C16.3295 14.0127 17.1045 12.121 17.1045 9.8877C17.1045 7.65436 16.3295 5.7627 14.7795 4.2127C13.2295 2.6627 11.3378 1.8877 9.10449 1.8877C6.87116 1.8877 4.97949 2.6627 3.42949 4.2127C1.87949 5.7627 1.10449 7.65436 1.10449 9.8877C1.10449 12.121 1.87949 14.0127 3.42949 15.5627C4.97949 17.1127 6.87116 17.8877 9.10449 17.8877Z" fill="#252432"></path>
                </svg>`
                        : actionItem.Title === 'Update'
                            ? `<svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M1.71999 18.5932C1.25966 18.5932 0.875326 18.439 0.566992 18.1307C0.258659 17.8224 0.104492 17.438 0.104492 16.9777V4.20871C0.104492 3.74838 0.258659 3.36404 0.566992 3.05571C0.875326 2.74738 1.25966 2.59321 1.71999 2.59321H10.1065L9.10649 3.59321H1.71999C1.56599 3.59321 1.42491 3.65729 1.29674 3.78546C1.16858 3.91363 1.10449 4.05471 1.10449 4.20871V16.9777C1.10449 17.1317 1.16858 17.2728 1.29674 17.401C1.42491 17.5291 1.56599 17.5932 1.71999 17.5932H14.489C14.643 17.5932 14.7841 17.5291 14.9122 17.401C15.0404 17.2728 15.1045 17.1317 15.1045 16.9777V9.48946L16.1045 8.48946V16.9777C16.1045 17.438 15.9503 17.8224 15.642 18.1307C15.3337 18.439 14.9493 18.5932 14.489 18.5932H1.71999ZM6.10449 12.5932V9.97771L15.0487 1.03371C15.1589 0.923378 15.2749 0.847045 15.3967 0.804712C15.5186 0.762379 15.6468 0.741211 15.7815 0.741211C15.9072 0.741211 16.0299 0.762379 16.1497 0.804712C16.2696 0.847045 16.3782 0.916961 16.4757 1.01446L17.5872 2.09321C17.6936 2.20354 17.7743 2.32505 17.8295 2.45771C17.8847 2.59038 17.9122 2.72404 17.9122 2.85871C17.9122 2.99321 17.892 3.12038 17.8515 3.24021C17.8112 3.36021 17.7359 3.47529 17.6257 3.58546L8.62374 12.5932H6.10449ZM7.10449 11.5932H8.19674L14.8622 4.92771L14.316 4.38171L13.7065 3.79696L7.10449 10.399V11.5932Z" fill="#252432"></path>
                    </svg>`
                            : actionItem.Title === 'Notes'
                                ? `<svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M3.60449 13.0928H9.60449V12.0928H3.60449V13.0928ZM3.60449 9.09277H12.6045V8.09277H3.60449V9.09277ZM3.60449 5.09277H12.6045V4.09277H3.60449V5.09277ZM1.71999 16.5928C1.25966 16.5928 0.875326 16.4386 0.566992 16.1303C0.258659 15.8219 0.104492 15.4376 0.104492 14.9773V2.20827C0.104492 1.74794 0.258659 1.36361 0.566992 1.05527C0.875326 0.74694 1.25966 0.592773 1.71999 0.592773H14.489C14.9493 0.592773 15.3337 0.74694 15.642 1.05527C15.9503 1.36361 16.1045 1.74794 16.1045 2.20827V14.9773C16.1045 15.4376 15.9503 15.8219 15.642 16.1303C15.3337 16.4386 14.9493 16.5928 14.489 16.5928H1.71999ZM1.71999 15.5928H14.489C14.643 15.5928 14.7841 15.5287 14.9122 15.4005C15.0404 15.2724 15.1045 15.1313 15.1045 14.9773V2.20827C15.1045 2.05427 15.0404 1.91319 14.9122 1.78502C14.7841 1.65686 14.643 1.59277 14.489 1.59277H1.71999C1.56599 1.59277 1.42491 1.65686 1.29674 1.78502C1.16858 1.91319 1.10449 2.05427 1.10449 2.20827V14.9773C1.10449 15.1313 1.16858 15.2724 1.29674 15.4005C1.42491 15.5287 1.56599 15.5928 1.71999 15.5928Z" fill="#252432"></path>
                        </svg>`
                                : `<svg width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M9.49774 9.56453C9.20629 9.56453 8.95217 9.4678 8.7354 9.27435C8.51862 9.0809 8.41024 8.85413 8.41024 8.59404C8.41024 8.33394 8.51862 8.10717 8.7354 7.91372C8.95217 7.72027 9.20629 7.62354 9.49774 7.62354C9.78919 7.62354 10.0433 7.72027 10.2601 7.91372C10.4768 8.10717 10.5852 8.33394 10.5852 8.59404C10.5852 8.85413 10.4768 9.0809 10.2601 9.27435C10.0433 9.4678 9.78919 9.56453 9.49774 9.56453ZM9.49774 16.358C7.28504 16.358 5.35889 15.7113 3.71931 14.418C2.07972 13.1246 1.13097 11.5068 0.873047 9.56453H1.97305C2.26867 11.2343 3.12372 12.6222 4.53819 13.7284C5.95267 14.8345 7.60585 15.3875 9.49774 15.3875C11.6184 15.3875 13.4173 14.7284 14.8945 13.4101C16.3716 12.0919 17.1102 10.4865 17.1102 8.59404C17.1102 6.70157 16.3716 5.09621 14.8945 3.77796C13.4173 2.4597 11.6184 1.80057 9.49774 1.80057C8.37254 1.80057 7.31503 2.01271 6.32523 2.43697C5.33524 2.86124 4.46171 3.4454 3.70462 4.18945H6.40271V5.15994H1.88524V1.12875H2.97274V3.44677C3.81356 2.62299 4.79892 1.98092 5.92883 1.52059C7.05893 1.06025 8.24856 0.830078 9.49774 0.830078C10.7049 0.830078 11.8355 1.03259 12.8896 1.43761C13.9438 1.84263 14.8651 2.39694 15.6535 3.10055C16.442 3.80416 17.0631 4.62625 17.517 5.56682C17.9708 6.50723 18.1977 7.51598 18.1977 8.59306C18.1977 9.66999 17.9708 10.6791 17.517 11.6203C17.0631 12.5615 16.442 13.3839 15.6535 14.0875C14.8651 14.7911 13.9438 15.3454 12.8896 15.7505C11.8355 16.1555 10.7049 16.358 9.49774 16.358Z" fill="#252432"></path>
                        </svg>`;

                    return `<a href="#" onclick="loadUpdateAndDetailModalPanel('${link}')" ${tooltipAttributes} ${dataAttributes} class="datatable-action ${cssClass}" ${customAttributes}>${svgIcon}${actionItem.LinkTitle}</a>`;
                } else {
                    return `<a href="#" onclick="loadUpdateAndDetailModalPanel('${link}')" ${tooltipAttributes} ${dataAttributes} class="datatable-action ${cssClass}" ${customAttributes}><i class="${this.actionIcons[actionItem.Title]}"></i>${actionItem.LinkTitle}</a>`;
                }
            }
            else if (actionItem.DatatableHrefType == "Link") {

                return `<a href="${link}" ${tooltipAttributes} ${dataAttributes} class="datatable-action ${cssClass}" ${customAttributes}> <i class="${this.actionIcons[actionItem.Title]}"></i>  ${actionItem.LinkTitle}</a >`;
            }
            else if (actionItem.DatatableHrefType == "Ajax") {
                return `<a href="#" onclick="sendAjaxRequest('${link}',this)" ${tooltipAttributes} ${dataAttributes} class="datatable-action ${cssClass}" ${customAttributes}> <i class="${this.actionIcons[actionItem.Title]}"></i>${actionItem.LinkTitle}</a>`
            }
        }
    }
    #getCellObjectValue(cellData, Prop) {
        if (Prop.indexOf(".") !== -1) {
            return cellData["" + Prop.split('.')[0]]["" + Prop.split('.')[1]];
        }
        else {
            return cellData['' + Prop];
        }
    }
    #renderHtml(data, dtColumns, meta, row) {
        if (dtColumns[meta.col].formatValue === "checkbox") {
            return '<input type="checkbox" class="checkbox-items ' + dtColumns[meta.col].className + '" value="' + data + '" />';
        }
        else if (dtColumns[meta.col].formatValue === "textbox") {
            return '<input type="text" class="text-box-items ' + dtColumns[meta.col].className + '" value="' + data + '" />';
        }
        else if (dtColumns[meta.col].formatValue === "number-textbox") {
            return '<input type="number" class="number-text-box-items ' + dtColumns[meta.col].className + '" value="' + data + '" />';
        }
        else if (dtColumns[meta.col].formatValue === "badge") {
            return '<span class="badge ' + data + '">' + data + '</span>';
        }
        else if (dtColumns[meta.col].formatValue === "link") {
            if (data !== null && data !== undefined && data !== "")
                return '<a href="' + data + '" target="_blank"><i class="fas fa-link"></i></a>';
            else
                return '<a onclick="return false;"><i class="fas fa-link disabled-link"></i></a>';
        }
        else if (dtColumns[meta.col].formatValue === "image") {
            if (data != "" && data != null && data != undefined) {
                return '<a href="' + data + '" target="_blank" class="rounded data-table-images' + ' ' + dtColumns[meta.col].className + '""><img class="photo" src="' + data + '" alt=""></a>';
            }
            else {
                return '<a href="/img/no-img.jpg" target="_blank" class="rounded data-table-images' + ' ' + dtColumns[meta.col].className + '"><img class="photo" src="/img/no-img.jpg" alt=""></a>';
            }
        }
        else if (dtColumns[meta.col].formatValue === "status") {
            let statusData = data?.replace(/\s+/g, '');
            return '<div class="dt-badge ' + statusData + '"><span class="custom-badge ' + statusData + '"></span><label>'+statusData+'</label></div>';
        }
        else if (dtColumns[meta.col].formatValue === "barcode") {
            return '<span data-barcode="' + data + '"><i class="fa fa-barcode"></i></span>';
        }
        else if (dtColumns[meta.col].formatValue === "detail") {
            return '<span class="details-control" data-url="' + dtColumns[meta.col].detailUrl + '">' + (data === undefined ? "" : data) + '</span>';
        }
        else if (dtColumns[meta.col].formatValue === "dropdown") {
            var statusOptions = dtColumns[meta.col].options;
            var dropdown = '<select class="form-select project-stage-status datatable-dropdown" attr-id="' + row.Id + '">';
            for (var i = 0; i < statusOptions.length; i++) {
                dropdown += '<option value="' + statusOptions[i].value + '"';
                if (statusOptions[i].value === data) {
                    dropdown += ' selected';
                }
                dropdown += '>' + statusOptions[i].text + '</option>';
            }
            dropdown += '</select>';
            return dropdown;
        }
        else if (dtColumns[meta.col].formatValue === "hidden") {
            return '<div>' + data + '</div><input type="hidden" class="hidden ' + dtColumns[meta.col].className + '" value="' + data + '">';
        }
        else if (dtColumns[meta.col].formatValue === "hidden-div") {
            return '<input type="hidden" class="hidden ' + dtColumns[meta.col].className + '" value="' + data + '">';
        }
        else if (dtColumns[meta.col].formatValue === "multiple-images") {
            if (Array.isArray(data)) {
                var imagesHtml = '';
                imagesHtml += '<div class="images-div">'
                for (var i = 0; i < data.length; i++) {
                    if (data[i] != "") {
                        if (i > 2) {
                            imagesHtml =
                                imagesHtml + `<a class="imgbadge">+${data.length - 3}</a>`;
                            return imagesHtml;
                        }

                        imagesHtml =
                            imagesHtml +
                            ` <a href="${data[i]}" target="_blank" class="rounded data-table-images"><img class="photo" src="${data[i]}" alt="" /></a>`;
                    }
                }
                imagesHtml += '</div>'
                return imagesHtml;
            }
            else {
                return '';
            }
        }
        else if (dtColumns[meta.col].formatValue === "tooltip") {
            if (data == null || data == undefined) {
                return "";
            }
            const hasTooltip = data?.length > 30;
            const tooltipContent = `data-bs-toggle="tooltip" data-bs-placement="top" title="${this.#encodeHTML(data)}"`;
            return `<span class="tooltip-wide ${this.#encodeHTML(data)}" ${tooltipContent}>${(this.#encodeHTML(data)?.slice(0, 30)) ?? "-"}${hasTooltip ? "..." : ""}</span>`;
        }
    }
    #encodeHTML = (html) => {
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
    };
    #renderNumericValue(data, dtColumns, meta) {
        if (dtColumns[meta.col].formatValue !== null && dtColumns[meta.col].formatValue !== "") {
            return data.toFixed(2);
        }
        return data.toFixed(dtColumns[meta.col].formatValue);
    }
    #setSearchFilters(showSelectedFilters) {
        var containerHtml = "";
        if (showSelectedFilters) {
            $('.search-filter-container input, .search-filter-container select').each(
                function (index) {
                    var input = $(this);
                    if (input.attr('type') != "hidden" && input.val() != "") {
                        var value = input.val();
                        var inputName = input.attr('name');
                        if ($(input).data('select2')) {
                            value = $(input).select2('data')[0].text
                        }
                        else if ($(input).is('select')) {
                            value = $(input).find(":selected").text();
                        }
                        else if ($(input).is(':checkbox')) {
                            value = $(input).is(":checked");
                        }
                        if (containerHtml == "") {
                            containerHtml = "<span class='me-1'>Filters: </span>";
                        }
                        containerHtml += "<span class='badge badge-datatable-search mb-1' data-input-name='" + inputName + "'>" + $(input).parent().find("label").html() + " : " + value + "</span>";
                    }
                }
            );
            if (containerHtml != "") {
                containerHtml += "<span class='datatable-clear-container'><span class='badge badge-datatable-clear fas mb-1'>Clear</span></span>";
                containerHtml = "<div class='d-flex flex-wrap w-100'>" + containerHtml + "</div>";
            }
            $(".selected-filters-container").html(containerHtml);
        }
        else {
            $(".selected-filters-container").html("");
        }
    }
    clearDatatableSearch() {
        $('#' + this.formId).trigger("reset");
        // In case of saved filters load. Reset does not work because it thinks the values that were loaded are the default values
        // Reset all input fields
        $('#' + this.formId).find('input').each(function () {
            switch (this.type) {
                case 'text':
                case 'search':
                case 'hidden':
                    $(this).val('');
                    break;
                case 'checkbox':
                case 'radio':
                    this.checked = false;
                    break;
            }
        });

        // Reset all textarea elements
        $('#' + this.formId).find('textarea').each(function () {
            $(this).val('');
        });

        $('select[class*="select2"]').each(function (i, element) {
            $(element).val('').trigger('change');
        });
        this.searchDataTable();
    }
    #removeSearchFilterInput(name) {
        var input = $("#" + this.formId + " input[name=" + name + "]");
        if ($(input).data('select2')) {
            $(input).val('').trigger('change');
        }
        else {
            $(input).val("");
        }
        this.searchDataTable();
    }
    // Its public because right now DeleteDataItem is Using it. We can make it private once we make those function inside this class as well
    searchDataTable() {
        // $(".list-container").show();
        //$(".search-filter-container").hide();
        // $("#" + this.tableId).dataTable().fnDestroy();
        // this.setupDataTable();
        this.dataTable.ajax.reload();
    }


}
function selectAllCheckBoxChanged(element) {
    let dataTable = $(element).closest(".dataTables_scrollHead").siblings(".dataTables_scrollBody").find("table").DataTable();

    if ($(element).is(":checked")) {
        dataTable.rows().select();   // Select all rows
    } else {
        dataTable.rows().deselect(); // Deselect all rows
    }
}
function CallBackFunctionality() {
}
CallBackFunctionality.prototype.GetFunctionality = function () {
    return "";
}
function getFinalCellClasses(cssClass, cellData) {
    if (cssClass != null) {
        const placeholders = cssClass.match(/@(\w+)/g); // Extract all placeholders
        if (placeholders) {
            placeholders.forEach(placeholder => {
                const propertyName = placeholder.slice(1); // Remove the "@" symbol
                if (cellData.hasOwnProperty(propertyName)) {
                    cssClass = cssClass.replace(new RegExp(placeholder, 'g'), cellData[propertyName]);
                }
            });
        }
        return cssClass;
    }
    else {
        return "";
    }
}

function getValueFromKeyString(obj, keyStr) {
    const keys = keyStr.replace(/\[(\d+)\]/g, '.$1')
        .replace(/\['([^']+)'\]/g, '.$1')
        .split('.');

    // Traverse the object using the keys
    return keys.reduce((acc, key) => acc && acc[key], obj);
}
function createEditableColumn(row, cellData, column, entityId) {
    const additionalFields = column.editableColumnDetail.additionalFields.map(field => {
        const propertyName = field.Item1;
        let identifierPath = "row." + field.Item2;
        const value = eval(identifierPath);
        return { [propertyName]: value };
    });
    const serializedAdditionalFields = JSON.stringify(additionalFields);
    const { cellName, entityName, fieldName, entityProperty, hiddenFormId, renderTooltip } = column.editableColumnDetail;
    return `attr-cell-name='${cellName}' attr-additional-fields='${serializedAdditionalFields}' attr-cell-data='${cellData}' attr-field-name='${fieldName}' attr-entity-name='${entityName}' attr-entity-property='${entityProperty}' attr-entity-id='${entityId}' attr-entity-is-tooltip='${renderTooltip}' attr-entity-form-id='${hiddenFormId}' `
}
function DeleteItem(deleteObj, dtObj) {
    $("#crudDeleteModal").modal("show");
    $("#user-password").val('');
    $("#crudDeleteModalError").html("");
    // Auto-focus on the password field when the modal is shown
    $('#crudDeleteModal').on('shown.bs.modal', function () {
        $("#user-password").focus();
    });

    // Submit the form when the user presses enter in the password field
    $(document).off("keyup", '#validate-password');
    $(document).on("keyup", '#validate-password', function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            $("#validate-password").click();
        }
    });
    $(document).off('click', '#validate-password');
    $(document).on('click', '#validate-password', function () {
        var password = $("#user-password").val();
        if (password != null && password != "") {
            $.ajax({
                url: "/Home/ValidatePassword",
                type: "post",
                data: { password: password },
                success: function (response) {
                    if (response) {
                        DeleteItem(deleteObj.deleteUrl).then(function (ajaxResult) {
                            if (ajaxResult.Success) {
                                $("#crudDeleteModal").modal("hide");
                                $("#user-password").val('');
                                if (ajaxResult.ReloadDatatable) {
                                    dtObj.searchDataTable();
                                }
                                else {
                                    if (deleteReturnUrl === "" || deleteReturnUrl === null || deleteReturnUrl === undefined) {
                                        location.reload();
                                    }
                                    else {
                                        window.location.href = deleteReturnUrl;
                                    }
                                }

                            }
                            else {
                                $("#crudDeleteModalError").html("<div class='alert alert-danger' role='alert'>Couldn't delete. Try again later.</div>");
                                $("#user-password").val('');
                                $("#user-password").focus();
                            }
                        });
                    }
                    else {
                        $("#crudDeleteModalError").html("<div class='alert alert-danger' role='alert'>You've entered a wrong password.</div>");
                        $("#user-password").val('');
                        $("#user-password").focus();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                }
            });
        }
        else {
            $("#crudDeleteModalError").html("<div class='alert alert-danger' role='alert'>Please enter the password.</div>");
        }

    });
}

function getColumnsToExport(idx, data, node) {
    if (shouldColumnBeIgnoredForExport(node)) {
        return false;
    }
    return true;
}
function shouldColumnBeIgnoredForExport(element) {
    if ($(element).hasClass("include-in-export"))
        return false;
    else if ($(element).hasClass("exclude-from-export") || $(element).is(":visible") === false)
        return true;
    return false;

}
function shouldBeIgnoredInPdfExport(element) {
    let th = $(element).closest('table').find('th').eq($(element).index())
    return shouldColumnBeIgnoredForExport(th);
}
function customizePdfExport(doc) {
    var colCount = new Array();
    $(dataTable).find('tbody tr:first-child td').each(function () {
        if (!shouldBeIgnoredInPdfExport($(this))) {
            if ($(this).attr('colspan')) {
                for (var i = 1; i <= $(this).attr('colspan'); $i++) {
                    colCount.push('*');
                }
            } else {
                colCount.push('*');
            }
        }

    });
    if (colCount.length < 8)// Cuts columns for table with more than 8 columns
        doc.content[1].table.widths = colCount;
    var rowCount = dataTable[0].rows.length;
    for (i = 0; i < rowCount; i++) {
        for (j = 0; j < colCount.length; j++) {
            doc.content[1].table.body[i][j].alignment = 'center';
        }

    };
}
function createEditableDatatableColumn(dataKey, cellName, entityName, hiddenFormId, renderCallBack = null, renderTooltip = false) {
    return {
        data: dataKey,
        export: false,
        orderable: true,
        className: "editable-cell",
        createdCell: function (cell, cellData, rowData, rowIndex, colIndex) {
            $(cell).attr("attr-cell-name", cellName);
            $(cell).attr("attr-cell-data", cellData);
            $(cell).attr("attr-entity-name", entityName);
            $(cell).attr("attr-entity-id", rowData.id);
            $(cell).attr("attr-entity-is-tooltip", renderTooltip);
            $(cell).attr("attr-entity-form-id", hiddenFormId);
        },
        render: renderCallBack == null ? function (data, type, row, meta) {
            return wrapHtmlSpan(data);
        } : function (data, type, row, meta) {
            return renderCallBack(data, type, row, meta);
        }
    };
}
function GetCellObjectValue(cellData, Prop) {
    if (Prop.indexOf(".") !== -1) {
        return cellData["" + Prop.split('.')[0]]["" + Prop.split('.')[1]];
    }
    else {
        return cellData['' + Prop];
    }
}

