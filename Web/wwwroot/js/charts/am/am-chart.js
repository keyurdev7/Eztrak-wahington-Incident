var roots = [];
const colorArray = [
    "#1F2937",    // Navy/Dark Blue
    "#E6E6E6",     // Pale Gray
    "#F4A593",    // Coral/Salmon Pink
    "#F2E5D7",    // Light Beige
    "#EBE2D8",    // Off-white/Cream
    "#E5E7EB",    // Light Gray
    "#D1D5DB",    // Medium Gray
    "#F5E6D3",    // Another shade of beige
]
const categoryFieldName = "Category";
const valueFieldName = "Value";

function GenerateAmPieChart(id, seriesData, title = "") {
    am5.ready(function () {
        DisposeRoot(id);
        var root = am5.Root.new(id);
        if (roots.find(x => x.type == id) === undefined) {
            roots.push({ type: id, root: root });
        }

        root.setThemes([
            am5themes_Animated.new(root)
        ]);

        var chart = root.container.children.push(
            am5percent.PieChart.new(root, {
                endAngle: 270,
                centerX: am5.percent(10),
                radius: am5.percent(70)
            })
        );

        var series = chart.series.push(
            am5percent.PieSeries.new(root, {
                valueField: valueFieldName,
                categoryField: categoryFieldName,
                endAngle: 270,
                alignLabels: false,
                legendLabelText: "{Category} :",
                legendValueText: "{Value}"
            })
        );

        // Add rounded corners and spacing between segments
        series.slices.template.setAll({
            cornerRadius: 10,
            stroke: am5.color(0xffffff),
            strokeWidth: 4,
            tooltipText: "{category}: {FormattedValue}"
        });

        // Hide default labels
        series.labels.template.set("forceHidden", true);
        series.ticks.template.set("forceHidden", true);
        
        // Custom colors for each slice based on category
        series.slices.template.adapters.add("fill", function(fill, target) {
            var dataItem = target.dataItem;
            if (dataItem) {
                var category = dataItem.get(categoryFieldName);
                // Map categories to specific colors - adjust based on your actual categories
                var colorMap = {
                    "Open": "#1F2937",       // Dark blue/navy
                    "Approved": "#e9967a",   // Salmon/coral
                    "Working": "#F2E5D7",    // Light green
                    "On Hold": "#f8c0c0",    // Light pink
                    "Complete": "#ffe4c4"    // Light peach
                };
                
                // If the category exists in our map, use that color
                if (colorMap[category]) {
                    return am5.color(colorMap[category]);
                }
            }
            return fill;
        });
        
        // Add custom labels inside slices
        series.bullets.push(function(root, series, dataItem) {

            var value = dataItem?.dataContext[valueFieldName];
            var category = dataItem?.dataContext[categoryFieldName];
            
            // Determine appropriate text color (white for dark backgrounds, black for light)
            var textColor = category === "Open" ? "#ffffff" : "#000000";
            
            // Create container for our labels
            var container = am5.Container.new(root, {});
            
            // Value text
            container.children.push(
                am5.Label.new(root, {
                    text: value ? value.toString():'',
                    fontSize: 14,
                    fontWeight: "bold",
                    fill: am5.color(textColor),
                    centerX: am5.p50,
                    centerY: am5.p50,
                    dy: -10
                })
            );
            
            // Category text
            container.children.push(
                am5.Label.new(root, {
                    text: category,
                    fontSize: 12,
                    fill: am5.color(textColor),
                    centerX: am5.p50,
                    centerY: am5.p50,
                    dy: 10
                })
            );
            
            return am5.Bullet.new(root, {
                sprite: container,
                locationX: 0.5,
                locationY: 0.5
            });
        });

        series.states.create("hidden", {
            endAngle: -90
        });

        // Set data
        series.data.setAll(seriesData);
        
        // var legend = chart.children.push(am5.Legend.new(root, {
        //     centerX: am5.percent(100),
        //     x: am5.percent(110),
        //     y: am5.percent(100),
        //     centerY: am5.percent(100),
        //     marginTop: 15,
        //     marginBottom: 15,
        //     layout: root.verticalLayout,
        //     nameField: categoryFieldName,
        //     valueField: valueFieldName,
        //     labelText: "{name}: {value}"
        // }));
        
        // legend.data.setAll(series.dataItems);
        series.appear(1000, 100);
    }); // end am5.ready()
}

function GenerateAmDoughnutChart(id, seriesData, title = '', setCustomDoughnutChartSeriesColor = null) {
    am5.ready(function () {
        DisposeRoot(id);
        var root = am5.Root.new(id);

        if (roots.find(x => x.type == id) === undefined) {
            roots.push({ type: id, root: root });
        }

        root.setThemes([
            am5themes_Animated.new(root)
        ]);

        // Create container for the whole chart
        var container = root.container.children.push(
            am5.Container.new(root, {
                width: am5.percent(100),
                height: am5.percent(100),
                layout: root.horizontalLayout,
                paddingRight: 10,
                paddingLeft: 10
            })
        );

        // Create chart container with fixed width
        var chartContainer = container.children.push(
            am5.Container.new(root, {
                width: am5.percent(60), // Allocate 60% of space for chart
                height: am5.percent(100),
                layout: root.horizontalLayout
            })
        );

        // Create the chart inside chartContainer
        var chart = chartContainer.children.push(
            am5percent.PieChart.new(root, {
                radius: am5.percent(85),
                innerRadius: am5.percent(50),
                centerY: am5.percent(0),
                centerX: am5.percent(50), // Center in its container
            })
        );
        if (!seriesData || seriesData.length === 0) {
            addEmptyState(chart, root, container, title);
            return;
        }
        var series = chart.series.push(
            am5percent.PieSeries.new(root, {
                valueField: valueFieldName,
                categoryField: categoryFieldName,
                alignLabels: true,
                legendLabelText: "{category}: {value}",
                legendValueText: "{value}",
                radius: am5.percent(85),
                innerRadius: am5.percent(30),
                strokeWidth: 0
            })
        );

        series.labels.template.set("forceHidden", true);
        // series.ticks.template.set("forceHidden", true);

        series.slices.template.set("tooltipText", "{category}: {FormattedValue}"); // Tooltip text
        series.states.create("hidden", {
            endAngle: -90
        });

        // Set custom colors for doughnut chart series
        setDoughnutChartSeriesColors(series);

        // Create legend container with fixed width
        var legendContainer = container.children.push(
            am5.Container.new(root, {
                width: am5.percent(40), // Allocate 40% of space for legend
                height: am5.percent(100),
                layout: root.verticalLayout,
                paddingLeft: 20
            })
        );

        // Add chart title above legend
        legendContainer.children.push(
            am5.Label.new(root, {
                text: title.length > 13 ? title.slice(0, 14) + '...' + title.slice(14) : title,
                fontSize: 20,
                fontWeight: "bold",
                textAlign: "left",
                width: am5.percent(100),
                x: am5.percent(0),
                y: am5.percent(15),
                centerX: am5.percent(0),
                marginBottom: 20,
                wrap: true,
                breakWords: true,
                oversizedBehavior: "wrap"
            })
        );

        // Add legend inside legendContainer
        var legend = legendContainer.children.push(
            am5.Legend.new(root, {
                // centerY: am5.percent(60),
                y: am5.percent(30),
                layout: root.verticalLayout,
                height: am5.percent(100),
                nameField: categoryFieldName,
                valueField: valueFieldName,
                verticalScrollbar: am5.Scrollbar.new(root, {
                    orientation: "vertical"
                }),
                labelText: "{name}: {value}"
            })
        );

        // Style legend markers
        legend.markers.template.setAll({
            width: 16,
            height: 16,
            cornerRadius: 0,
            verticalCenter: "top"
        });

        // Configure legend labels
        legend.labels.template.setAll({
            fontSize: 13,
            fontWeight: "400",
            paddingLeft: 5,
            width: 150,
            lineHeight: 1.2,
            oversizedBehavior: "wrap",
            textAlign: "left",
            wrap: true,
            breakWords: false
        });

        // Style value labels
        legend.valueLabels.template.setAll({
            fontSize: 14,
            fontWeight: "400",
            paddingLeft: 10,
            verticalCenter: "top"
        });

        // Adjust legend item spacing
        legend.itemContainers.template.setAll({
            paddingTop: 5,
            paddingBottom: 5,
            marginTop: 2,
            marginBottom: 2
        });

        // Set data
        series.data.setAll(seriesData);
        legend.data.setAll(series.dataItems);

        // Add animation
        series.appear(1000, 100);
    });
}

function setDoughnutChartSeriesColors(series) {
    series.slices.template.adapters.add("fill", function (fill, target) {
        // Get the series index
        const seriesIndex = series.slices.indexOf(target);

        // Check if the index is within the bounds of colorArray
        if (seriesIndex >= 0 && seriesIndex < colorArray.length) {
            return am5.color(colorArray[seriesIndex]);
        }
    });

    series.slices.template.adapters.add("stroke", function (stroke, target) {
        // Get the series index
        const seriesIndex = series.slices.indexOf(target);

        // Check if the index is within the bounds of colorArray
        if (seriesIndex >= 0 && seriesIndex < colorArray.length) {
            return am5.color(colorArray[seriesIndex]);
        }
    });
}

function GenerateAmBarChart(id, seriesData, setCustomBarChartSeriesColor = null) {
    am5.ready(function () {

        DisposeRoot(id);
        var root = am5.Root.new(id);
        if (roots.find(x => x.type == id) === undefined) {
            roots.push({ type: id, root: root });
        }
        // Set themes
        // https://www.amcharts.com/docs/v5/concepts/themes/
        root.setThemes([
            am5themes_Animated.new(root)
        ]);


        // Create chart
        // https://www.amcharts.com/docs/v5/charts/xy-chart/
        var chart = root.container.children.push(am5xy.XYChart.new(root, {
            panX: true,
            panY: true,
            wheelX: "panX",
            wheelY: "zoomX",
            pinchZoomX: true,
            layout: root.verticalLayout
        }));


        // Add Export Options
        let exporting = am5plugins_exporting.Exporting.new(root, {
            menu: am5plugins_exporting.ExportingMenu.new(root, {}),
            dataSource: seriesData,
            filePrefix: "data_chart"
        });
        // Add cursor
        // https://www.amcharts.com/docs/v5/charts/xy-chart/cursor/
        var cursor = chart.set("cursor", am5xy.XYCursor.new(root, {}));
        cursor.lineY.set("visible", false);


        // Create axes
        // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
        var xRenderer = am5xy.AxisRendererX.new(root, { minGridDistance: 30 });
        xRenderer.labels.template.setAll({
            rotation: -90,
            centerY: am5.p50,
            centerX: am5.p100,
            paddingRight: 15
        });

        xRenderer.grid.template.setAll({
            location: 1
        })

        var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
            maxDeviation: 0.3,
            categoryField: categoryFieldName,
            renderer: xRenderer,
            tooltip: am5.Tooltip.new(root, {})
        }));

        var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
            maxDeviation: 0.3,
            renderer: am5xy.AxisRendererY.new(root, {
                strokeOpacity: 0.1
            })
        }));


        // Create series
        // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
        var series = chart.series.push(am5xy.ColumnSeries.new(root, {
            name: "Series 1",
            xAxis: xAxis,
            yAxis: yAxis,
            valueYField: valueFieldName,
            sequencedInterpolation: true,
            categoryXField: categoryFieldName,
            tooltip: am5.Tooltip.new(root, {
                labelText: "{valueY}"
            }),
            // legendLabelText: "Series: {name}",
            legendLabelText: "{category}: {value}",
            maskBullets: false
        }));

        series.columns.template.setAll({ cornerRadiusTL: 5, cornerRadiusTR: 5, strokeOpacity: 0 });
        setBarChartToolTip(series, root, chart);
        //setBarChartSeriesBullets(series, root);
        series.columns.template.label = am5.Label.new(root, {
            text: "{valueY}",
            centerX: am5.p50,
            centerY: am5.p50,
            fontSize: 12,
            fill: "white",
        });
        if (setCustomBarChartSeriesColor && typeof setCustomBarChartSeriesColor === 'function') {
            setCustomBarChartSeriesColor(series);
        } else {
            // If series is longer than colorArray, add missing colors
            for (let i = colorArray.length; i < seriesData.length; i++) {
                colorArray.push(getRandomColor());
            }
            setBarChartSeriesColors(series);
        }

        xAxis.data.setAll(seriesData);
        xAxis.get("renderer").labels.template.setAll({
            oversizedBehavior: "truncate",
            textAlign: "center",
            maxHeight: 100
        });
        series.data.setAll(seriesData);

        chart.children.push(am5.Legend.new(root, {
            centerX: am5.percent(50),
            x: am5.percent(50),
            y: am5.percent(50),
            centerY: am5.percent(50),
            layout: root.verticalLayout,
            nameField: categoryFieldName,
            valueField: valueFieldName,
            labelText: "{name}: {value}"
        }));

        // Make stuff animate on load
        // https://www.amcharts.com/docs/v5/concepts/animations/
        series.appear(1000);
        chart.appear(1000, 100);
    });
}

// Function to generate a random hex color
function getRandomColor() {
    return colorArray[Math.floor(Math.random() * colorArray.length)];
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

function setBarChartSeriesColors(series) {
    series.columns.template.adapters.add("fill", function (fill, target) {
        // Get the series index
        const seriesIndex = series.columns.indexOf(target);

        // Check if the index is within the bounds of colorArray
        if (seriesIndex >= 0 && seriesIndex < colorArray.length) {
            return am5.color(colorArray[seriesIndex]);
        }
    });

    series.columns.template.adapters.add("stroke", function (stroke, target) {
        // Get the series index
        const seriesIndex = series.columns.indexOf(target);

        // Check if the index is within the bounds of colorArray
        if (seriesIndex >= 0 && seriesIndex < colorArray.length) {
            return am5.color(colorArray[seriesIndex]);
        }
    });
}
function setBarChartSeriesBullets(series, root) {
    series.bullets.push(function () {
        return am5.Bullet.new(root, {
            locationX: 0.5,
            locationY: 0.5,
            sprite: am5.Label.new(root, {
                text: "{Value}",
                fill: root.interfaceColors.get("alternativeText"),
                centerX: am5.percent(50),
                centerY: am5.percent(50),
                populateText: true
            })
        });
    });
}
function setBarChartToolTip(series, root, barChart) {
    var tooltip = series.set("tooltip", am5.Tooltip.new(root, {
        getFillFromSprite: false,
        getStrokeFromSprite: true,
        autoTextColor: false,
        pointerOrientation: "horizontal"
    }));

    tooltip.get("background").setAll({
        fill: am5.color(0xffffff),
        fillOpacity: 0.8
    });

    tooltip.get("background").setAll({
        fill: am5.color(0xffffff)
    })

    tooltip.label.setAll({
        text: "{Category}[/]",
        fill: am5.color(0x000000)
    });

    tooltip.label.adapters.add("text", function (text, target) {
        return getBarChartToolTipContent(barChart, text);
    });
}
function getBarChartToolTipContent(barChart, text) {
    barChart.series.each(function (series) {
        //text += '\n[' + series.get("stroke").toString() + ']●[/] [bold width:100px]' + series.get("name") + ':[/] {' + series.get("valueYField") + '}'
        text += ': {' + series.get("valueYField") + '}'
    })
    return text;
}
function DisposeRoot(root) {
    if (roots.length > 0) {
        if (root !== undefined && root !== "") {
            am5.array.each(am5.registry.rootElements, function (r) {
                if (r !== undefined && r.dom.id == root) {
                    r.dispose();
                }
            });
        }
        //else
        //    roots.forEach(function (v, i) {
        //        if (!$.isEmptyObject(v.root))
        //            v.root.dispose();
        //    })
    }
}

function addEmptyState(chart, root, container, title = '') {
    // Clear any existing series
    chart.series.clear();

    // Create empty state series
    var emptyState = chart.series.push(
        am5percent.PieSeries.new(root, {
            valueField: "value",
            categoryField: "category",
            radius: am5.percent(85),
            innerRadius: am5.percent(30)
        })
    );

    // Set a single data item for the empty state
    emptyState.data.setAll([{
        category: "No Data",
        value: 100
    }]);

    // Style the empty state
    emptyState.slices.template.setAll({
        fill: am5.color(0xeeeeee),
        stroke: am5.color(0xdadada),
        strokeWidth: 1,
        strokeOpacity: 0.5,
        fillOpacity: 0.7,
        interactive: false,
        tooltipText: ""
    });

    // Add "No Data" label in center of chart
    chart.seriesContainer.children.push(
        am5.Label.new(root, {
            text: "No Data",
            textAlign: "center",
            centerY: am5.percent(50),
            centerX: am5.percent(50),
            fontSize: 16,
            fill: am5.color(0x999999)
        })
    );

    // Create legend container with fixed width
    var legendContainer = container.children.push(
        am5.Container.new(root, {
            width: am5.percent(40),
            height: am5.percent(100),
            layout: root.verticalLayout,
            paddingLeft: 20
        })
    );

    // Add chart title above legend
    legendContainer.children.push(
        am5.Label.new(root, {
            text: title.length > 13 ? title.slice(0, 14) + '...' + title.slice(14) : title,
            fontSize: 20,
            fontWeight: "bold",
            textAlign: "left",
            width: am5.percent(100),
            x: am5.percent(0),
            y: am5.percent(15),
            centerX: am5.percent(0),
            marginBottom: 20,
            wrap: true,
            breakWords: true,
            oversizedBehavior: "wrap"
        })
    );

    return emptyState;
}