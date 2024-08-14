//var app = angular.module("test", []);

//app.controller("testcontroller", function ($scope) {
//    $scope.data = "New Data";
//});
//app.directive("NewDir", function () {
//    return {
//        restrict: "E",
//        Scope: {},
//        template: "",
//        templateurl: "",
//        transcude: true,
//        controller: function (scope) {
//        },
//        link : function(scope, controller, element)
//        {
//        }
//    }

//});

app.filter("CommaSeperater",
    function() {
        return function(value) {
            if (value != undefined) {
                return value.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");
            } else {
                return 0.00 .toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");
            }
        };
    });

app.filter("CommaSeperaterInt",
    function() {
        return function(value) {
            if (value != undefined) {
                return value.toFixed(0).replace(/./g,
                    function(c, i, a) {
                        return i > 0 && c !== "." && (a.length - i) % 3 === 0 ? `,${c}` : c;
                    });
            } else {
                return (0.0).toFixed(0).replace(/./g,
                    function(c, i, a) {
                        return i > 0 && c !== "." && (a.length - i) % 3 === 0 ? `,${c}` : c;
                    });
            }
        };
    });

function formatDecimalInComma(value) {
    if (value != undefined) {
        return value.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");
    } else {
        return 0.00 .toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");
    }
}