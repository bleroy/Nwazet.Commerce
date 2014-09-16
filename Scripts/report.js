$(function() {
    var cutoff = 31;
    var ctx = $("#report-chart").get(0).getContext("2d");
    var dataTable = $("#commerce-report-data-table");
    var labels = dataTable
        .find("tbody tr td.description")
        .map(function() {
            return $(this).html();
        });
    var values = dataTable
        .find("tbody tr td.value")
        .map(function() {
            return Math.round(parseFloat($(this).data("value"), 10) * 100) / 100;
        });
    var series = dataTable
        .find("thead tr th.series-description")
        .map(function() {
            return $(this).html();
        });
    var seriesData = $.map(series, function(val, index) {
        return dataTable
            .find("tbody tr")
            .map(function() {
                return Math.round(parseFloat(
                    $(this)
                    .find("td.series-value")
                    .eq(index)
                    .data("value"), 10) * 100) / 100;
            });
    });
    var chartType = dataTable.data("chart-type");
    var palette = [
        "hsla(164,34%,50%,1)",
        "hsla(141,20%,64%,1)",
        "hsla(34,54%,85%,1)",
        "hsla(42,72%,68%,1)",
        "hsla(7,68%,54%,1)",
        "hsla(13,54%,33%,1)"
    ];
    var otherText = dataTable.data("other-text");
    var sum = function(array) {
        var s = 0;
        for (var i = 0; i < array.length; i++) {
            s += array[i];
        }
        return s;
    };
    var appendIfHasValue = function(array, item) {
        if (item.value) {
            array.push(item);
        }
        return array;
    };
    var data = chartType === "Doughnut"
        ? appendIfHasValue(
            $.map(values.slice(0, cutoff), function(value, index) {
                return {
                    value: value,
                    color: palette[index % palette.length],
                    label: labels[index]
                };
            }), {
                value: sum(values.slice(cutoff)),
                color: palette[cutoff % palette.length],
                label: otherText
            })
        : series.length == 0
        ? {
            labels: labels,
            datasets: [
                {
                    label: document.title,
                    fillColor: "rgba(220,220,220,0.2)",
                    strokeColor: "rgba(220,220,220,1)",
                    pointColor: "rgba(220,220,220,1)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgba(220,220,220,1)",
                    data: values
                }
            ]
        }
        : {
            labels: labels,
            datasets: $.map(seriesData.slice(0, cutoff), function(val, index) {
                var color = palette[index % palette.length];
                return {
                    label: series[index],
                    fillColor: "rgba(0,0,0,0)",
                    strokeColor: color,
                    pointColor: color,
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: color,
                    data: seriesData[index]
                };
            })
        };
    new Chart(ctx)[chartType](data, { bezierCurve: false });
    dataTable
        .find("thead tr th.series-description")
        .each(function(index) {
            $(this)
                .prepend($("<span></span>")
                    .css({
                        width: "8px",
                        height: "8px",
                        backgroundColor: palette[index % palette.length],
                        display: "inline-block",
                        marginRight: "4px"
                    }));
        });

    $("#startDate,#endDate").calendarsPicker({
        showAnim: "",
        renderer: $.extend({}, $.calendars.picker.themeRollerRenderer, {
            picker: "<div {popup:start} id='ui-datepicker-div'{popup:end} class='ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all{inline:start} ui-datepicker-inline{inline:end}'><div class='ui-datepicker-header ui-widget-header ui-helper-clearfix ui-corner-all'>{link:prev}{link:today}{link:next}</div>{months}{popup:start}{popup:end}<div class='ui-helper-clearfix'></div></div>",
            month: "<div class='ui-datepicker-group'><div class='ui-datepicker-month ui-helper-clearfix'>{monthHeader:MM yyyy}</div><table class='ui-datepicker-calendar'><thead>{weekHeader}</thead><tbody>{weeks}</tbody></table></div>"
        })
    });
});