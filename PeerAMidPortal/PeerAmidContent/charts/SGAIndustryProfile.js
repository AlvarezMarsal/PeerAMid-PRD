//plotIndustryProfile();

function plotIndustryProfile(wcd) {
    //console.log("Entered plotIndustryProfile");
    const chart = AmCharts.makeChart("divChartSGAIndustryProfile",
        {
            "type": "serial",
            "startDuration": 0,
            "columnWidth": 0.95,
            "depth3D": 0,
            "autoMarginOffset": 20,
            "categoryField": "Category",
            "categoryAxis": {
                "axisColor": "#AAB3B3",
                "autoRotateAngle": -90,
                "autoRotateCount": 1,
                "autoWrap": true,
                "gridPosition": "start",
                "gridThickness": 0,
                "gridAlpha": 1,
                "tickPosition": "start",
                "dashLength": 8,
                "title": ".",
                "titleBold": false,
                "titleColor": "#FFFFFF",

                "guides": [
                    {
                        "category": "Food, Beverage & Tobacco",

                        "lineColor": "#C00000",
                        "lineAlpha": 1,
                        "fillAlpha": 0.2,
                        "fillColor": "#ffffff",
                        "lineThickness": 35,
                        "dashLength": 4,
                        "inside": true,
                    },
                    {
                        "category": "Food, Beverage & Tobacco",

                        "lineColor": "#ffffff",
                        "lineAlpha": 1,
                        "fillAlpha": 0.2,
                        "fillColor": "#ffffff",
                        "lineThickness": 30,
                        "dashLength": 0,
                        "inside": true,
                    }
                ]
            },
            "trendLines": [],
            "graphs": [
                {
                    "precision": 1,

                    "closeField": "Median",
                    "fillAlphas": 1,
                    "fillColors": "#002060",
                    "lineColor": "#c3c3c3",
                    "lineThickness": 1,
                    "id": "AmGraph-1",
                    "openField": "P25",
                    "title": "graph 1",
                    "type": "column",
                    "labelText": "[[P25]]%",
                    "balloonText": "[[P25]]%",
                    "fontSize": 9,
                    "showAllValueLabels": true,
                    "labelPosition": "inside",
                    "labelOffset": -15,
                    "lineColorField": "line1",
                    "dashLengthField": "dLF",
                },
                {
                    "precision": 1,

                    "closeField": "P75",
                    "fillAlphas": 1,
                    "fillColors": "#92d050",
                    "lineColor": "#c3c3c3",
                    "lineThickness": 1,
                    "id": "AmGraph-2",
                    "openField": "Median",
                    "title": "graph 2",
                    "type": "column",
                    "labelText": "[[Median]]%",
                    "balloonText": "[[Median]]%",
                    "labelOffset": -2,
                    "labelPosition": "inside",
                    "fontSize": 9,
                    "showAllValueLabels": true,
                    "lineColorField": "line1",
                    "dashLengthField": "dLF",
                    "bulletOffset": -25,
                    "bulletSize": 24,
                    "customBulletField": "bullet",
                },
                {
                    "precision": 1,

                    "fillAlphas": 0,
                    "lineThickness": 0,
                    "id": "AmGraph-3",
                    "title": "graph 3",
                    "labelText": "[[P75]]%",
                    "balloonText": "[[P75]]%",
                    "valueField": "P75",
                    "fontSize": 9,
                    "showAllValueLabels": true,
                }
            ],
            "valueAxes": [
                {
                    "axisColor": "#AAB3B3",
                    "offset": 1,
                    "strictMinMax": true,
                    "minimum": 0,
                    "maximum": 62,
                    "id": "ValueAxis-1",
                    "stackType": "regular",
                    "gridThickness": 0,
                    "unit": "%",
                }
            ],

            "allLabels": [],
            "balloon": {},

            "dataProvider": [
                {
                    "P75": 59.1,
                    "Median": 52.3,
                    "P25": 44.4,
                    "Category": "Bank",
                    "dLF": 0
                },
                {
                    "P75": 51.4,
                    "Median": 35.2,
                    "P25": 22.7,
                    "Category": "Household & Personal \nProducts",
                    "dLF": 0
                },
                {
                    "P75": 47.0,
                    "Median": 31.0,
                    "P25": 19.9,
                    "Category": "Software & Services",
                    "dLF": 0
                },
                {
                    "P75": 52.5,
                    "Median": 29.1,
                    "P25": 16.1,
                    "Category": "Diversified Financials",

                    "dLF": 0
                },
                {
                    "P75": 35.4,
                    "Median": 27.6,
                    "P25": 21.0,
                    "Category": "Retailing",
                    "dLF": 0
                },
                {
                    "P75": 38.8,
                    "Median": 29.3,
                    "P25": 21.1,
                    "Category": "Pharma, Biotech & \nLife Sciences",
                    //"line1": "#ff0000",//To change by Dharmesh: As suggested by client to hide the red hashed rectangle!
                    "line1": "#c3c3c3",
                    //"dLF": 4 //To hide the dotted rectangle!
                    "dLF": 0
                    //, "bullet": "../Content/Images/star-label.png"
                },
                {
                    "P75": 36.8,
                    "Median": 28.3,
                    "P25": 16.8,
                    "Category": "Telecommunication \nServices",
                    "dLF": 0
                },
                {
                    "P75": 35.2,
                    "Median": 23.5,
                    "P25": 10.8,
                    "Category": "Healthcare Equipment & \nServices",
                    "dLF": 0
                },
                {
                    "P75": 33.3,
                    "Median": 22.9,
                    "P25": 12.9,
                    "Category": "Consumer Durable & \nApparel",
                    "dLF": 0
                },
                {
                    "P75": 29.1,
                    "Median": 21.1,
                    "P25": 12.5,
                    "Category": "Media",
                    "dLF": 0
                },
                {
                    "P75": 28.7,
                    "Median": 20.7,
                    "P25": 12.9,
                    "Category": "Commercial & Professiol \nServices",
                    "dLF": 0
                },
                {
                    "P75": 23.6,
                    "Median": 20.0,
                    "P25": 12.4,
                    "Category": "Food & Staples Retailing",
                    "dLF": 0
                },
                {
                    "P75": 27.4,
                    "Median": 19.3,
                    "P25": 9.3,
                    "Category": "Food, Beverage & Tobacco",
                    "dLF": 0
                },
                {
                    "P75": 29.4,
                    "Median": 18.0,
                    "P25": 11.0,
                    "Category": "Technology Hardware & \nEquipment",
                    "dLF": 0
                },
                {
                    "P75": 21.0,
                    "Median": 16.9,
                    "P25": 11.9,
                    "Category": "Semiconductors& Semi. \nEquipment",
                    "dLF": 0
                },
                {
                    "P75": 26.5,
                    "Median": 16.2,
                    "P25": 9.7,
                    "Category": "Consumer Services",
                    "dLF": 0
                },
                {
                    "P75": 21.5,
                    "Median": 15.6,
                    "P25": 9.6,
                    "Category": "Capital Goods",
                    "dLF": 0
                },
                {
                    "P75": 23.2,
                    "Median": 15.5,
                    "P25": 9.4,
                    "Category": "Insurance",
                    "dLF": 0
                },
                {
                    "P75": 16.7,
                    "Median": 10.4,
                    "P25": 5.0,
                    "Category": "Energy",
                    "dLF": 0
                },
                {
                    "P75": 15.2,
                    "Median": 9.1,
                    "P25": 5.5,
                    "Category": "Materials",
                    "dLF": 0
                },
                {
                    "P75": 12.6,
                    "Median": 8.0,
                    "P25": 6.5,
                    "Category": "Automobiles & \nComponents",
                    "dLF": 0
                },
                {
                    "P75": 11.2,
                    "Median": 7.2,
                    "P25": 4.7,
                    "Category": "Real Estate",
                    "dLF": 0
                },
                {
                    "P75": 13.0,
                    "Median": 5.5,
                    "P25": 3.8,
                    "Category": "Transportation",
                    "dLF": 0
                },
                {
                    "P75": 14.5,
                    "Median": 5.6,
                    "P25": 1.8,
                    "Category": "Utilities",
                    "dLF": 0
                },
                {
                    "P75": 29.8,
                    "Median": 17.3,
                    "P25": 8.9,
                    "Category": "Cross Industry",
                    "dLF": 0
                }
            ] /*,
            "listeners": [
                {
                    "event": "drawn",
                    "method": function (e) { console.log("plotIndustryProfile drawn event"); }
                },
                {
                    "event": "animationFinished",
                    "method": function (e) { console.log("plotIndustryProfile  animationFinished event"); }
                },
                {
                    "event": "rendered",
                    "method": function (e) { console.log("plotIndustryProfile  rendered event"); }
                }
            ]*/

        });

    //console.log("Exited plotIndustryProfile");
}