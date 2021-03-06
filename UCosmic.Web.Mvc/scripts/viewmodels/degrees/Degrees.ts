module ViewModels.Degrees {

    export class DegreeSearchInput {
        personId: number;
        orderBy: string;
        pageSize: number;
        pageNumber: number;
    }

    export class DegreeList {
        personId: number;
        orderBy: string;
        pageSize: number;
        pageNumber: number;
        items: KnockoutObservableArray<any>;

        constructor(personId: number) {
            this.personId = personId;
        }

        load(): JQueryPromise<any> {
            var deferred: JQueryDeferred<void> = $.Deferred();
            var expertiseSearchInput: DegreeSearchInput = new DegreeSearchInput();

            expertiseSearchInput.orderBy = "";
            expertiseSearchInput.pageNumber = 1;
            expertiseSearchInput.pageSize = App.Constants.int32Max;

            $.get(App.Routes.WebApi.My.Degrees.get(), expertiseSearchInput)
                .done((data: any, textStatus: string, jqXHR: JQueryXHR): void => {
                    ko.mapping.fromJS(data, {}, this);
                    deferred.resolve();
                })
                .fail((xhr: JQueryXHR): void => {
                    App.Failures.message(xhr, 'while loading your degrees', true);
                });

            return deferred;
        }

        deleteEducationById(expertiseId: number): void {
            $.ajax({
                async: false,
                type: "DELETE",
                url: App.Routes.WebApi.My.Degrees.del(expertiseId),
                success: (data: any, textStatus: string, jqXHR: JQueryXHR): void =>
                { },
                error: (xhr: JQueryXHR): void => {
                    App.Failures.message(xhr, 'while deleting your degree', true);
                }
            });
        }

        deleteEducation(data: any, event: any, viewModel: any): void {
            $("#confirmDegreeDeleteDialog").dialog({
                dialogClass: 'jquery-ui',
                width: 'auto',
                resizable: false,
                modal: true,
                buttons: [
                    {
                        text: "Yes, confirm delete", click: function (): void {
                            viewModel.deleteEducationById(data.id());
                            $(this).dialog("close");

                            /* TBD - Don't reload page. */
                            location.href = App.Routes.Mvc.My.Profile.get("geographic-expertise");
                        }
                    },
                    {
                        text: "No, cancel delete", click: function (): void {
                            $(this).dialog("close");
                        }
                    },
                ]
            });
        }

        editUrl(id: number): string {
            return App.Routes.Mvc.My.Degrees.edit(id);
        }
    }
}
