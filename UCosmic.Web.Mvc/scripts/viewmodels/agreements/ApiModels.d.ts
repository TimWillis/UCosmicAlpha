declare module Agreements.ApiModels {

    export interface FlatEstablishment {
        id: number;
        officialName: string;
        translatedName: string;
        officialUrl: string;
        countryName: string;
        countryCode: string;
        uCosmicCode: string;
        ceebCode: string;
    }

    export interface ScalarEstablishment {
        id: number;
        typeId: number;
        uCosmicCode: string;
        ceebCode: string;
    }

    export interface NameInput {
        id: number;
        ownerId: number;
        text: string;
        isOfficialName: boolean;
        isFormerName: boolean;
        languageCode: string;
    }

    export interface Name extends NameInput {
        languageName: string;
    }

    export interface Url {
        id: number;
        ownerId: number;
        value: string;
        isOfficialUrl: boolean;
        isFormerUrl: boolean;
    }

    export interface Location {
        center: Places.ApiModels.Point;
        box: Places.ApiModels.Box;
        googleMapZoomLevel?: number;
        places: Places.ApiModels.Place[];
    }

    export interface PutLocation {
        center?: Places.ApiModels.Point;
        box?: Places.ApiModels.Box;
        googleMapZoomLevel?: number;
        placeId?: number;
    }

    export interface Participant {
        agreementId: number;
        agreementType: string;
        agreementStartsOn: string;
        establishmentId: number;
        isOwner: boolean;
        establishmentOfficialName: string;
        establishmentTranslatedName: string;
        center: Places.ApiModels.Point;
        boundingBox: Places.ApiModels.Box;
        googleMapZoomLevel: number;
    }

    export interface PlaceWithAgreements {
        id?: number;
        continentId?: number;
        continentCode?: string;
        countryId?: number;
        countryCode?: string;
        name?: string;
        type?: string;
        agreementIds?: number[]
        agreementCount?: number;
        partnerIds?: number[]
        partnerCount?: number;
        isEarth?: boolean;
        isContinent?: boolean;
        isCountry?: boolean;
        isAdmin1?: boolean;
        isAdmin2?: boolean;
        isAdmin3?: boolean;
        center?: Places.ApiModels.Point;
        boundingBox?: Places.ApiModels.Box;
    }

    export interface Summary {
        agreementCount: number;
        partnerCount: number;
        countryCount: number;
    }
}

declare module Agreements.KoModels {
    export interface Summary {
        agreementCount: KnockoutObservable<string>;
        partnerCount: KnockoutObservable<string>;
        countryCount: KnockoutObservable<string>;
    }
}