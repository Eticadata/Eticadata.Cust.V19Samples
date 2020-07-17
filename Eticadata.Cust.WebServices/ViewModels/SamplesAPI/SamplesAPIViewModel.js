var SamplesAPIViewModel = function (divDefaultView) {
    var self = this;
    self.divDefaultView = divDefaultView;

    var myRibbon = {
        title: 'Exemplos API', ribbon: [{

            //Create a group
            title: getMsgTranslated("TrainingAction", "titleTables"),
            items: [
                //Add itens to ribbon group
                { title: getMsgTranslated("TrainingAction", "btnGenerateItem"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "1000001", enabled: true },
                { title: getMsgTranslated("TrainingAction", "btnGenerateCustomer"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "1000002", enabled: true },
            ]
        },
        {
            title: getMsgTranslated("TrainingAction", "titleMovements"),
            items: [
                { title: getMsgTranslated("TrainingAction", "btnGenerateOrder"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "10000010", enabled: true },
                { title: getMsgTranslated("TrainingAction", "btnGenerateSale"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "10000011", enabled: true },                
                { title: getMsgTranslated("TrainingAction", "btnGenerateRepairOrder"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "1000013", enabled: true },
                { title: getMsgTranslated("TrainingAction", "btnGenerateSettlement"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "1000014", enabled: true },
                { title: getMsgTranslated("TrainingAction", "btnGenerateWorkSheet"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "1000012", enabled: true },
            ]
        },
        {
            title: getMsgTranslated("TrainingAction", "titleSatisfaction"),
            items: [
                { title: getMsgTranslated("TrainingAction", "btnSatisfactionOrder"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "1000020", enabled: true },
                { title: getMsgTranslated("TrainingAction", "btnSatisfactionWorkSheet"), image: "Geral/Comandos/WebService", itemType: "LIT", commType: "JS", commName: "1000021", enabled: true },

            ]
        },
        ]
    };

    //Add contextual ribbon
    MyShell().Ribbon.UpdateContextualTab(myRibbon, GetIFrameKey(window.frameElement.id), function (commName, params) {

        if (commName == "1000001") {
            self.GenerateItem();
        }

        if (commName == "1000002") {
            self.GenerateCustomer();
        }

        if (commName == "10000010") {
            self.GenerateCustomerOrder();
        }

        if (commName == "10000011") {
            self.GenerateSalesDoc();
        }

        if (commName == "1000013") {
            self.GenerateRepairOrder();
        }

        if (commName == "1000014") {
            self.GenerateSettlement();
        }
        
        if (commName == "1000020") {
            self.GenerateOrdersSatisfaction();
        }


    }, function () {
        MyShell().Ribbon.CloseWebTab();
    });

    self.GenerateOrdersSatisfaction = function () {
        MyShell().Dialog.showWaitingBox();

        var sample = {
            DocTypeAbbrevSale: "FAT", FiscalYearSale: "2018", DateSale: new Date(), ExpirationDateSale: new Date(),
            KeyOrder: {FiscalYear: "2018", SectionCode: "1", DocTypeAbbrev: "ENCCL", Number: 1 }            
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: GetUrlBase() + 'api/OrdersSatisfactionTA/GenerateOrdersSatisfaction',
            dataType: "json",
            data: ko.toJSON(sample),

            success: function (d) {
                MyShell().Dialog.closeWaitingBox();
                
                if (d.ErrorDescription == "") {
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), getMsgTranslated("msgBoxGravar", "Criação do documento de ordem de reparação efetuada com sucesso!"), getMsgTranslated("generic", "OK"));
                }
                else {
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), d.ErrorDescription, getMsgTranslated("generic", "OK"));
                }
            },

            error: function (d) {
                MyShell().Dialog.closeWaitingBox();               
                showErrorAjax(d);
            }

        });
    }


    self.GenerateRepairOrder = function () {
        MyShell().Dialog.showWaitingBox();

        var sample = {
            FiscalYear: "2018", SectionCode: "1", DocTypeAbbrev: "OR", EntityCode: 1, Date: new Date(), Vehicle: "VG-00-01",
            LinesMaterials: [{ LineNumber: 1, ItemCode: "ART1", ItemDescription: "Artigo linha material", Quantity: 3, VATTax: 23, UnitPriceExcludedVAT: 100 }],
            LinesInternalServices: [{ LineNumber: 1, ItemCode: "MO", ItemDescription: "Artigo linha serv. interno", Quantity: 4, VATTax: 23, UnitPriceExcludedVAT: 120 }],
            LinesExternalServices: [{ LineNumber: 1, ItemCode: "MO", ItemDescription: "Artigo linha serv. externo", Quantity: 5, VATTax: 23, UnitPriceExcludedVAT: 130 }]
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: GetUrlBase() + 'api/RepairOrdersTA/GenerateRepairOrder',
            dataType: "json",
            data: ko.toJSON(sample),

            success: function (d) {
                MyShell().Dialog.closeWaitingBox();

                if (d.ErrorDescription == "") {
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), getMsgTranslated("msgBoxGravar", "Criação do documento de ordem de reparação efetuada com sucesso!"), getMsgTranslated("generic", "OK"));
                }
                else {                    
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), d.ErrorDescription, getMsgTranslated("generic", "OK"));
                }
            },

            error: function (d) {                
                MyShell().Dialog.closeWaitingBox();
                showErrorAjax(d);
            }

        });
    }


    self.GenerateSettlement = function () {
        MyShell().Dialog.showWaitingBox();

        var sample = {
            DocTypeAbbrev: "REC", SectionCode: "1",
            PendingDocument: { FiscalYear: "2018", SectionCode: "1", DocTypeAbbrev: "FAT", Number: 1}, GetReportBytes: true
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: GetUrlBase() + 'api/SettlementTA/GenerateSettlement',
            dataType: "json",
            data: ko.toJSON(sample),

            success: function (d) {
                MyShell().Dialog.closeWaitingBox();

                if (d.ErrorDescription == "") {
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), getMsgTranslated("msgBoxGravar", "Criação do documento de liquidação efetuada com sucesso!"), getMsgTranslated("generic", "OK"));
                }
                else {
                     MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), d.ErrorDescription, getMsgTranslated("generic", "OK"));
                }
            },

            error: function (d) {
                MyShell().Dialog.closeWaitingBox();
                showErrorAjax(d);
            }

        });
    }

    self.GenerateSalesDoc = function () {
        MyShell().Dialog.showWaitingBox();

        var sample = {
            FiscalYear: "2018", SectionCode: "1", DocTypeAbbrev: "FAT", EntityCode: 1, Date: new Date(), ExpirationDate: new Date(), CurrencyCode: "EUR",
            Lines: [{ LineNumber: 1, ItemCode: "ART1", ItemDescription: "Artigo cust vnd", Quantity: 3, VATTax: 23, UnitPriceExcludedVAT: 100, Discount1: 1, Discount2: 2, Discount3: 3, DiscountValue: 4 }],
            LinesPayment: [{ PayMovTypeCodeTres: "CHQ", Value: 300, CurrencyCode: "EUR", Exchange: 1 }, {PayMovTypeCodeTres: "NUM", Value: 32.51, CurrencyCode: "EUR", Exchange: 1}],
            GetReportBytes: true
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: GetUrlBase() + 'api/SalesTA/GenerateSalesDoc',
            dataType: "json",
            data: ko.toJSON(sample),

            success: function (d) {
                MyShell().Dialog.closeWaitingBox();

                if (d.ErrorDescription == "") {
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), getMsgTranslated("msgBoxGravar", "Criação do documento de venda efetuada com sucesso!"), getMsgTranslated("generic", "OK"));
                }
                else {
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), d.ErrorDescription, getMsgTranslated("generic", "OK"));
                }
            },

            error: function (d) {
                MyShell().Dialog.closeWaitingBox();
                showErrorAjax(d);
            }

        });
    }

    self.GenerateCustomerOrder = function () {
        MyShell().Dialog.showWaitingBox();

        var sample = {
            FiscalYear: "2018", SectionCode: "1", DocTypeAbbrev: "ENCCL", EntityCode: 1, Date: new Date(), ExpirationDate: new Date(), CurrencyCode: "EUR",
            Lines: [{ LineNumber: 1, ItemCode: "ART1", ItemDescription: "Artigo cust", Quantity: 3, VATTax: 23, UnitPriceExcludedVAT: 100, Discount1: 1, Discount2: 2, Discount3: 3, DiscountValue: 4 }], GetReportBytes: true
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: GetUrlBase() + 'api/CustomerOrdersTA/GenerateCustomerOrder',
            dataType: "json",
            data: ko.toJSON(sample),

            success: function (d) {
                MyShell().Dialog.closeWaitingBox();

                if (d.ErrorDescription == "") {
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), getMsgTranslated("msgBoxGravar", "Criação do encomenda a cliente efetuada com sucesso!"), getMsgTranslated("generic", "OK"));
                }
                else {
                    MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), d.ErrorDescription, getMsgTranslated("generic", "OK"));
                }
            },

            error: function (d) {
                MyShell().Dialog.closeWaitingBox();
                showErrorAjax(d);
            }

        });
    }

    self.GenerateItem = function () {
        MyShell().Dialog.showWaitingBox();

        var sample = { Code: "ART2", Category: "1", Description: "Artigo 2", Abbreviation: "AR 2", VATRateSale: 3, VATRatePurchase: 3, MeasureOfStock: "UN", MeasureOfSale: "UN", MeasureOfPurchase: "UN" }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: GetUrlBase() + 'api/ItemsTA/GenerateItem',
            dataType: "json",
            data: ko.toJSON(sample),

            success: function (d) {
                MyShell().Dialog.closeWaitingBox();
                MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), getMsgTranslated("msgBoxGravar", "Criação do artigo efetuada com sucesso!"), getMsgTranslated("generic", "OK"));
            },

            error: function (d) {
                MyShell().Dialog.closeWaitingBox();
                showErrorAjax(d);
            }

        });
    }

    self.GenerateCustomer = function () {
        MyShell().Dialog.showWaitingBox();

        var sample = { GenerateNewCode: true, Code: 0, Name: "Cliente", AddressLine1: "Morada linha 1", AddressLine2: "Morada linha 2", Locality: "Localidade", PostalCode: "4170-505", Email: "email@eticadata.pt", SubZone: "BRG", PaymentTerm: 1 };

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: GetUrlBase() + 'api/CustomerTA/GenerateCustomer',
            dataType: "json",
            data: ko.toJSON(sample),

            success: function (d) {
                MyShell().Dialog.closeWaitingBox();
                MyShell().Dialog.showMessageBox(450, 200, "info", getMsgTranslated("generic", "ERP eticadata"), getMsgTranslated("msgBoxGravar", "Criação do cliente efetuada com sucesso!"), getMsgTranslated("generic", "OK"));
            },

            error: function (d) {
                MyShell().Dialog.closeWaitingBox();
                showErrorAjax(d);
            }

        });
    }
};