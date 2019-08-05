/*global module*/
(function () {
    angular
        .module("simplAdmin.paymentPaypalSmart")
        .factory("paypalSmartService", paypalSmartService);
    /* @ngInject */
    function paypalSmartService($http) {
        var service = {
            getSettings: getSettings,
            setSettings: setSettings
        };
        return service;

        function getSettings() {
            return $http.get('api/paypal-smart/config');
        }

        function setSettings(settings) {
            return $http.put('api/paypal-smart/config', settings);
        }
    }
})();
