require([
    "viewmodels/agreements/AgreementVM"
], function (Search) {
    var establishmentSearchViewModel = new Search.Search();
    ko.applyBindings(establishmentSearchViewModel, $('#main')[0]);
    establishmentSearchViewModel.sammy.run();
});
