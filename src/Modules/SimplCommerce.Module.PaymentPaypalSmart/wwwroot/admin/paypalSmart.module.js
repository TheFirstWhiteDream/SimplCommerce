/*global angular*/
(function () {
    'use strict';

    angular
        .module('simplAdmin.paymentPaypalSmart', [])
        .config(['$stateProvider',
            function ($stateProvider) {
                $stateProvider
                    .state('payments-paypalsmart-config', {
                        url: '/payments/paypal-smart/config',
                        templateUrl: 'modules/paymentpaypalsmart/admin/config/config-form.html',
                        controller: 'paypalSmartConfigFormCtrl as vm'
                    })
                    ;
            }
        ]);
})();
