define(["require", "exports", '../amd-modules/Establishments/SearchResult', '../amd-modules/Establishments/Search'], function(require, exports, __SearchResultModule__, __SearchModule__) {
    function InstitutionalAgreementParticipantModel(js) {
        var self = this;
        ko.mapping.fromJS(js, {
        }, self);
        self.isNotOwner = ko.computed(function () {
            return !self.isOwner();
        });
        self.participantEl = undefined;
    }
    var SearchResultModule = __SearchResultModule__;

    var SearchModule = __SearchModule__;

    var SearchResult = SearchResultModule.SearchResult;
    var Search = SearchModule.Search;
    function InstitutionalAgreementEditModel() {
        var self = this;
        self.isBound = ko.observable();
        self.back = function () {
            history.back();
        };
        self.sideSwiper = new App.SideSwiper({
            speed: '',
            frameWidth: 970,
            root: '[data-current-module=agreements]'
        });
        self.participants = ko.observableArray();
        var owner;
        var tenantDomain = $('#tenancy_domain').val();
        var homeParticipant = new InstitutionalAgreementParticipantModel({
            isOwner: true,
            establishment: new SearchResult({
                id: 1,
                officialName: 'University of Cincinnati',
                translatedName: 'University of Cincinnati',
                officialUrl: 'www.uc.edu',
                countryName: 'United States',
                countryCode: 'asdf',
                uCosmicCode: 'asdf',
                ceebCode: 'asdf'
            }, owner)
        });
        if(tenantDomain === 'usf.edu') {
            homeParticipant.establishment = new SearchResult({
                id: 1,
                officialName: 'University of South Florida',
                translatedName: 'University of South Florida',
                officialUrl: 'www.usf.edu',
                countryName: 'United States',
                countryCode: 'asdf',
                uCosmicCode: 'asdf',
                ceebCode: 'asdf'
            }, owner);
        }
        if(tenantDomain === 'lehigh.edu') {
            homeParticipant.establishment = new SearchResult({
                id: 1,
                officialName: 'Lehigh University',
                translatedName: 'Lehigh University',
                officialUrl: 'www.lehigh.edu',
                countryName: 'United States',
                countryCode: 'asdf',
                uCosmicCode: 'asdf',
                ceebCode: 'asdf'
            }, owner);
        }
        if(tenantDomain === 'umn.edu') {
            homeParticipant.establishment = new SearchResult({
                id: 1,
                officialName: 'University of Minnesota',
                translatedName: 'University of Minnesota',
                officialUrl: 'www.umn.edu',
                countryName: 'United States',
                countryCode: 'asdf',
                uCosmicCode: 'asdf',
                ceebCode: 'asdf'
            }, owner);
        }
        if(tenantDomain === 'uc.edu') {
            homeParticipant.establishment = new SearchResult({
                id: 1,
                officialName: 'University of Cincinnati',
                translatedName: 'University of Cincinnati',
                officialUrl: 'www.uc.edu',
                countryName: 'United States',
                countryCode: 'asdf',
                uCosmicCode: 'asdf',
                ceebCode: 'asdf'
            }, owner);
        }
        if(tenantDomain === 'suny.edu') {
            homeParticipant.establishment = new SearchResult({
                id: 1,
                officialName: 'State University of New York',
                translatedName: 'State University of New York',
                officialUrl: 'www.suny.edu',
                countryName: 'United States',
                countryCode: 'asdf',
                uCosmicCode: 'asdf',
                ceebCode: 'asdf'
            }, owner);
        }
        self.participants.push(homeParticipant);
        self.establishmentSearchViewModel = new Search();
        self.establishmentSearchViewModel.sammy = undefined;
        self.establishmentSearchViewModel.nextPage = function () {
            if(self.establishmentSearchViewModel.nextEnabled()) {
                var pageNumber = parseInt(self.establishmentSearchViewModel.pageNumber()) + 1;
                location.hash = '/participants/add/page/' + pageNumber;
            }
        };
        self.establishmentSearchViewModel.prevPage = function () {
            if(self.establishmentSearchViewModel.prevEnabled()) {
                history.back();
            }
        };
        self.addParticipant = function (establishmentResultViewModel) {
            var participant = new InstitutionalAgreementParticipantModel({
                isOwner: false,
                establishment: establishmentResultViewModel
            });
            self.participants.push(participant);
            location.hash = "#/";
        };
        self.establishmentSearchViewModel.items.subscribe(function (newValue) {
            if(newValue && newValue.length) {
                for(var i = 0; i < newValue.length; i++) {
                    if(newValue[i].clickAction !== self.addParticipant) {
                        newValue[i].clickAction = self.addParticipant;
                    }
                }
            }
        });
        self.removeParticipant = function (establishmentResultViewModel, e) {
            if(confirm('Are you sure you want to remove "' + establishmentResultViewModel.translatedName() + '" as a participant from this agreement?')) {
                self.participants.remove(function (item) {
                    if(item.establishment.id() === establishmentResultViewModel.id()) {
                        $(item.participantEl).slideUp('fast', function () {
                            self.participants.remove(item);
                        });
                    }
                    return false;
                });
            }
            e.preventDefault();
            e.stopPropagation();
            return false;
        };
        var dataSideSwiper = 'data-side-swiper';
        var sam = undefined;
        self.sammy = function () {
            if(sam) {
                return sam;
            }
            sam = Sammy(function () {
                this.get('#/', function () {
                    if($('#participants_add').attr(dataSideSwiper) === 'on') {
                        self.sideSwiper.prev();
                    }
                });
                this.get('#/participants/add/page/:pageNumber', function () {
                    if($('#all').attr(dataSideSwiper) === 'on') {
                        self.sideSwiper.next();
                        self.establishmentSearchViewModel.trail().push(this.path);
                    } else if($('#participants_add').attr(dataSideSwiper) === 'on') {
                        var pageNumber = this.params['pageNumber'], trail = self.establishmentSearchViewModel.trail(), clone;
                        self.establishmentSearchViewModel.pageNumber(pageNumber);
                        if(trail.length > 0 && trail[trail.length - 1] === this.path) {
                            return;
                        }
                        if(trail.length > 1 && trail[trail.length - 2] === this.path) {
                            trail.pop();
                            clone = self.establishmentSearchViewModel.$itemsPage().clone(true).removeAttr('data-bind').data('bind', undefined).removeAttr('id');
                            clone.appendTo(self.establishmentSearchViewModel.$itemsPage().parent());
                            self.establishmentSearchViewModel.$itemsPage().attr('data-side-swiper', 'off').hide();
                            self.establishmentSearchViewModel.lockAnimation();
                            $(window).scrollTop(0);
                            self.establishmentSearchViewModel.sideSwiper.prev(1, function () {
                                self.establishmentSearchViewModel.$itemsPage().siblings().remove();
                                self.establishmentSearchViewModel.unlockAnimation();
                            });
                            return;
                        } else if(trail.length > 0) {
                            clone = self.establishmentSearchViewModel.$itemsPage().clone(true).removeAttr('data-bind').data('bind', undefined).removeAttr('id');
                            clone.insertBefore(self.establishmentSearchViewModel.$itemsPage());
                            self.establishmentSearchViewModel.$itemsPage().attr('data-side-swiper', 'off').data('side-swiper', 'off').hide();
                            self.establishmentSearchViewModel.lockAnimation();
                            $(window).scrollTop(0);
                            self.establishmentSearchViewModel.sideSwiper.next(1, function () {
                                self.establishmentSearchViewModel.unlockAnimation();
                            });
                        }
                        trail.push(this.path);
                    }
                });
                this.get('', function () {
                    this.app.runRoute('get', '#/');
                });
            });
            return sam;
        };
        self.isBound(true);
    }
    exports.InstitutionalAgreementEditModel = InstitutionalAgreementEditModel;
})
