/*global module*/
(function () {
    'use strict';
    angular
        .module('simplAdmin.paymentPaypalSmart')
        .controller('paypalSmartConfigFormCtrl', paypalSmartConfigFormCtrl);

    /* @ngInject */
    function paypalSmartConfigFormCtrl($state, paypalSmartService, translateService) {
        var vm = this;
        vm.translate = translateService;
        vm.paypalSmartConfig = {};

        vm.save = function save() {
            vm.validationErrors = [];
            paypalSmartService.setSettings(vm.paypalSmartConfig)
                .then(function (result) {
                    toastr.success("config has been saved");
                }).catch(function (response) {
                    vm.validationErrors = [];
                    var errors = response.data;
                    if (error && angular.isObject(error)) {
                        for (var key in error) {
                            vm.validationErrors.push(error[key][0]);
                        }
                    } else {
                        vm.validationErrors.push('Could not save settings.');
                    }
                });
        };

        function init() {
            paypalSmartService.getSettings()
                .then(function (result) {
                    vm.paypalSmartConfig = result.data;
                });
        }

        init();
    }

})(jQuery);
