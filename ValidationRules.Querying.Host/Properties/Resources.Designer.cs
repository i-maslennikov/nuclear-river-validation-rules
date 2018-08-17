﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuClear.ValidationRules.Querying.Host.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuClear.ValidationRules.Querying.Host.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для оформления заказа недостаточно средств. Необходимо: {0}. Имеется: {1}..
        /// </summary>
        internal static string AccountBalanceShouldBePositive {
            get {
                return ResourceManager.GetString("AccountBalanceShouldBePositive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Заказ не имеет привязки к лицевому счёту..
        /// </summary>
        internal static string AccountShouldExist {
            get {
                return ResourceManager.GetString("AccountShouldExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Может быть выпущено количество позиций в месяц {3} - {4}.
        /// </summary>
        internal static string AdvertisementAmountShouldMeetMinimumRestrictions {
            get {
                return ResourceManager.GetString("AdvertisementAmountShouldMeetMinimumRestrictions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В рубрику {0} заказано слишком много объявлений: Заказано {1}, допустимо не более {2}..
        /// </summary>
        internal static string AdvertisementCountPerCategoryShouldBeLimited {
            get {
                return ResourceManager.GetString("AdvertisementCountPerCategoryShouldBeLimited", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Слишком много продаж в тематику {0}. Продано {1} позиций вместо {2} возможных..
        /// </summary>
        internal static string AdvertisementCountPerThemeShouldBeLimited {
            get {
                return ResourceManager.GetString("AdvertisementCountPerThemeShouldBeLimited", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} выбран рекламный материал {1}, не принадлежащий фирме {2}.
        /// </summary>
        internal static string AdvertisementMustBelongToFirm {
            get {
                return ResourceManager.GetString("AdvertisementMustBelongToFirm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Рекламный материал {0} ожидает модерацию.
        /// </summary>
        internal static string AdvertisementMustPassReview_Draft {
            get {
                return ResourceManager.GetString("AdvertisementMustPassReview_Draft", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Рекламный материал {0} не прошёл модерацию.
        /// </summary>
        internal static string AdvertisementMustPassReview_Invalid {
            get {
                return ResourceManager.GetString("AdvertisementMustPassReview_Invalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Рекламный материал {0} одобрен с замечаниями.
        /// </summary>
        internal static string AdvertisementShouldNotHaveComments_OkWithComment {
            get {
                return ResourceManager.GetString("AdvertisementShouldNotHaveComments_OkWithComment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Данные из системы AMS слишком старые, результаты проверок недостоверны.
        /// </summary>
        internal static string AmsMessagesShouldBeNew {
            get {
                return ResourceManager.GetString("AmsMessagesShouldBeNew", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} необходимо указать хотя бы один валидный адрес.
        /// </summary>
        internal static string AtLeastOneLinkedPartnerFirmAddressShouldBeValid {
            get {
                return ResourceManager.GetString("AtLeastOneLinkedPartnerFirmAddressShouldBeValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Отсутствует сканированная копия договора.
        /// </summary>
        internal static string BargainScanShouldPresent {
            get {
                return ResourceManager.GetString("BargainScanShouldPresent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для заказа необходимо сформировать счета.
        /// </summary>
        internal static string BillsShouldBeCreated {
            get {
                return ResourceManager.GetString("BillsShouldBeCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Сумма по счетам не совпадает с планируемой суммой заказа.
        /// </summary>
        internal static string BillsSumShouldMatchOrder {
            get {
                return ResourceManager.GetString("BillsSumShouldMatchOrder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Юр. лицо организации.
        /// </summary>
        internal static string BranchOffice {
            get {
                return ResourceManager.GetString("BranchOffice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Юр. лицо исполнителя.
        /// </summary>
        internal static string BranchOfficeOrganizationUnit {
            get {
                return ResourceManager.GetString("BranchOfficeOrganizationUnit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Валюта.
        /// </summary>
        internal static string Currency {
            get {
                return ResourceManager.GetString("Currency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для подразделения {0} установлено более одной тематики по умолчанию.
        /// </summary>
        internal static string DefaultThemeMustBeExactlyOne_Many {
            get {
                return ResourceManager.GetString("DefaultThemeMustBeExactlyOne_Many", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для подразделения {0} не указана тематика по умолчанию.
        /// </summary>
        internal static string DefaultThemeMustBeExactlyOne_None {
            get {
                return ResourceManager.GetString("DefaultThemeMustBeExactlyOne_None", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Установленная по умолчанию тематика {0} должна содержать только саморекламу.
        /// </summary>
        internal static string DefaultThemeMustHaveOnlySelfAds {
            get {
                return ResourceManager.GetString("DefaultThemeMustHaveOnlySelfAds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Позиция {0} оформлена на пустой адрес {1}.
        /// </summary>
        internal static string FirmAddressMustBeLocatedOnTheMap {
            get {
                return ResourceManager.GetString("FirmAddressMustBeLocatedOnTheMap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to На адрес {0} фирмы {1} продано более одной кнопки в заголовок карточки в периоды: {2}.
        /// </summary>
        internal static string FirmAddressMustNotHaveMultipleCallToAction {
            get {
                return ResourceManager.GetString("FirmAddressMustNotHaveMultipleCallToAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to На адрес {0} фирмы {1} продано более одной позиции &apos;Реклама в профилях партнёров&apos; в периоды: {2}.
        /// </summary>
        internal static string FirmAddressShouldNotHaveMultiplePartnerAdvertisement {
            get {
                return ResourceManager.GetString("FirmAddressShouldNotHaveMultiplePartnerAdvertisement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Отделение организации назначения заказа не соответствует отделению организации выбранной фирмы.
        /// </summary>
        internal static string FirmAndOrderShouldBelongTheSameOrganizationUnit {
            get {
                return ResourceManager.GetString("FirmAndOrderShouldBelongTheSameOrganizationUnit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1} &quot;{0}&quot; является сопутствующей, основная позиция не найдена..
        /// </summary>
        internal static string FirmAssociatedPositionMustHavePrincipal {
            get {
                return ResourceManager.GetString("FirmAssociatedPositionMustHavePrincipal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} &quot;{1}&quot; содержит объекты привязки, конфликтующие с объектами привязки следующей основной позиции: {2} &quot;{3}&quot;.
        /// </summary>
        internal static string FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject {
            get {
                return ResourceManager.GetString("FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} &quot;{1}&quot; содержит объекты привязки, конфликтующие с объектами привязки следующей основной позиции: {2} &quot;{3}&quot; в заказе {4}.
        /// </summary>
        internal static string FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject_Order {
            get {
                return ResourceManager.GetString("FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject_Order", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} &quot;{1}&quot; содержит объекты привязки, отсутствующие в основных позициях..
        /// </summary>
        internal static string FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject {
            get {
                return ResourceManager.GetString("FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} &quot;{1}&quot; данного Заказа является основной для следующих позиций: {2} &quot;{3}&quot;.
        /// </summary>
        internal static string FirmAssociatedPositionShouldNotStayAlone {
            get {
                return ResourceManager.GetString("FirmAssociatedPositionShouldNotStayAlone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} &quot;{1}&quot; является запрещённой для: {2} &quot;{3}&quot;.
        /// </summary>
        internal static string FirmPositionMustNotHaveDeniedPositions {
            get {
                return ResourceManager.GetString("FirmPositionMustNotHaveDeniedPositions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} &quot;{1}&quot; является запрещённой для: {2} &quot;{3}&quot; в заказе {4}.
        /// </summary>
        internal static string FirmPositionMustNotHaveDeniedPositions_Order {
            get {
                return ResourceManager.GetString("FirmPositionMustNotHaveDeniedPositions_Order", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для фирмы {0} задано слишком большое число рубрик - {1}. Максимально допустимое - {2}..
        /// </summary>
        internal static string FirmShouldHaveLimitedCategoryCount {
            get {
                return ResourceManager.GetString("FirmShouldHaveLimitedCategoryCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Юр. лицо заказчика.
        /// </summary>
        internal static string LegalPerson {
            get {
                return ResourceManager.GetString("LegalPerson", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Профиль юр. лица заказчика.
        /// </summary>
        internal static string LegalPersonProfile {
            get {
                return ResourceManager.GetString("LegalPersonProfile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to У юр. лица клиента, в профиле {0} указан договор с датой окончания действия раньше даты подписания заказа..
        /// </summary>
        internal static string LegalPersonProfileBargainShouldNotBeExpired {
            get {
                return ResourceManager.GetString("LegalPersonProfileBargainShouldNotBeExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to У юр. лица клиента, в профиле {0} указана доверенность с датой окончания действия раньше даты подписания заказа.
        /// </summary>
        internal static string LegalPersonProfileWarrantyShouldNotBeExpired {
            get {
                return ResourceManager.GetString("LegalPersonProfileWarrantyShouldNotBeExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to У юр. лица клиента отсутствует профиль..
        /// </summary>
        internal static string LegalPersonShouldHaveAtLeastOneProfile {
            get {
                return ResourceManager.GetString("LegalPersonShouldHaveAtLeastOneProfile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} найдена рубрика {1}, не принадлежащая адресу {2}.
        /// </summary>
        internal static string LinkedCategoryFirmAddressShouldBeValid {
            get {
                return ResourceManager.GetString("LinkedCategoryFirmAddressShouldBeValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} найдена неактивная рубрика {1}.
        /// </summary>
        internal static string LinkedCategoryShouldBeActive {
            get {
                return ResourceManager.GetString("LinkedCategoryShouldBeActive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} найдена рубрика {1}, не принадлежащая фирме заказа.
        /// </summary>
        internal static string LinkedCategoryShouldBelongToFirm {
            get {
                return ResourceManager.GetString("LinkedCategoryShouldBelongToFirm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} адрес фирмы {1} скрыт до выяснения.
        /// </summary>
        internal static string LinkedFirmAddressShouldBeValid_ClosedForAscertainment {
            get {
                return ResourceManager.GetString("LinkedFirmAddressShouldBeValid_ClosedForAscertainment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} адрес фирмы {1} скрыт навсегда.
        /// </summary>
        internal static string LinkedFirmAddressShouldBeValid_Deleted {
            get {
                return ResourceManager.GetString("LinkedFirmAddressShouldBeValid_Deleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} найден адрес {1}, привязанный к зданию со специальным назначением.
        /// </summary>
        internal static string LinkedFirmAddressShouldBeValid_InvalidBuildingPurpose {
            get {
                return ResourceManager.GetString("LinkedFirmAddressShouldBeValid_InvalidBuildingPurpose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} найден адрес {1}, не привязанный ко входу.
        /// </summary>
        internal static string LinkedFirmAddressShouldBeValid_MissingEntrance {
            get {
                return ResourceManager.GetString("LinkedFirmAddressShouldBeValid_MissingEntrance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} найден неактивный адрес {1}.
        /// </summary>
        internal static string LinkedFirmAddressShouldBeValid_NotActive {
            get {
                return ResourceManager.GetString("LinkedFirmAddressShouldBeValid_NotActive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} найден адрес {1}, не принадлежащий фирме заказа.
        /// </summary>
        internal static string LinkedFirmAddressShouldBeValid_NotBelongToFirm {
            get {
                return ResourceManager.GetString("LinkedFirmAddressShouldBeValid_NotBelongToFirm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Фирма {0} скрыта до выяснения..
        /// </summary>
        internal static string LinkedFirmShouldBeValid_ClosedForAscertainment {
            get {
                return ResourceManager.GetString("LinkedFirmShouldBeValid_ClosedForAscertainment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Фирма {0} скрыта навсегда.
        /// </summary>
        internal static string LinkedFirmShouldBeValid_ClosedForever {
            get {
                return ResourceManager.GetString("LinkedFirmShouldBeValid_ClosedForever", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Фирма {0} удалена..
        /// </summary>
        internal static string LinkedFirmShouldBeValid_Deleted {
            get {
                return ResourceManager.GetString("LinkedFirmShouldBeValid_Deleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Рекламный материал {0} (наличие необязательно) ожидает модерацию.
        /// </summary>
        internal static string OptionalAdvertisementMustPassReview_Draft {
            get {
                return ResourceManager.GetString("OptionalAdvertisementMustPassReview_Draft", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Рекламный материал {0} (наличие необязательно) не прошёл модерацию.
        /// </summary>
        internal static string OptionalAdvertisementMustPassReview_Invalid {
            get {
                return ResourceManager.GetString("OptionalAdvertisementMustPassReview_Invalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Указана некорректная дата начала размещения..
        /// </summary>
        internal static string OrderBeginDistrubutionShouldBeFirstDayOfMonth {
            get {
                return ResourceManager.GetString("OrderBeginDistrubutionShouldBeFirstDayOfMonth", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} необходимо указать рекламные материалы.
        /// </summary>
        internal static string OrderCheckPositionMustHaveAdvertisements {
            get {
                return ResourceManager.GetString("OrderCheckPositionMustHaveAdvertisements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} есть возможность указать рекламные материалы.
        /// </summary>
        internal static string OrderCheckPositionMustHaveOptionalAdvertisements {
            get {
                return ResourceManager.GetString("OrderCheckPositionMustHaveOptionalAdvertisements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Указана некорректная дата окончания размещения..
        /// </summary>
        internal static string OrderEndDistrubutionShouldBeLastSecondOfMonth {
            get {
                return ResourceManager.GetString("OrderEndDistrubutionShouldBeLastSecondOfMonth", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для заказа указана неактивная работа.
        /// </summary>
        internal static string OrderMustHaveActiveDeal_Inactive {
            get {
                return ResourceManager.GetString("OrderMustHaveActiveDeal_Inactive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для заказа не указана работа.
        /// </summary>
        internal static string OrderMustHaveActiveDeal_Missing {
            get {
                return ResourceManager.GetString("OrderMustHaveActiveDeal_Missing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Заказ ссылается на неактивные объекты: {0}.
        /// </summary>
        internal static string OrderMustHaveActiveLegalEntities {
            get {
                return ResourceManager.GetString("OrderMustHaveActiveLegalEntities", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Позиции не соответствуют актуальному прайс-листу. Необходимо указать позиции из текущего действующего прайс-листа..
        /// </summary>
        internal static string OrderMustHaveActualPrice {
            get {
                return ResourceManager.GetString("OrderMustHaveActualPrice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Заказ оформлен на период, по которому уже сформирована сборка. Необходимо указать другие даты размещения заказа..
        /// </summary>
        internal static string OrderMustNotIncludeReleasedPeriod {
            get {
                return ResourceManager.GetString("OrderMustNotIncludeReleasedPeriod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} задействованы рубрики, не привязанные к отделению организации города назначения заказа: .
        /// </summary>
        internal static string OrderMustUseCategoriesOnlyAvailableInProject {
            get {
                return ResourceManager.GetString("OrderMustUseCategoriesOnlyAvailableInProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} необходимо указать хотя бы один объект привязки для подпозиции &apos;{1}&apos;.
        /// </summary>
        internal static string OrderPositionAdvertisementMustBeCreated {
            get {
                return ResourceManager.GetString("OrderPositionAdvertisementMustBeCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} необходимо указать рекламные материалы для подпозиции &quot;{1}&quot;.
        /// </summary>
        internal static string OrderPositionAdvertisementMustHaveAdvertisement {
            get {
                return ResourceManager.GetString("OrderPositionAdvertisementMustHaveAdvertisement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В позиции {0} есть возможность указать рекламные материалы для подпозиции &quot;{1}&quot;.
        /// </summary>
        internal static string OrderPositionAdvertisementMustHaveOptionalAdvertisement {
            get {
                return ResourceManager.GetString("OrderPositionAdvertisementMustHaveOptionalAdvertisement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Позиция {0} соответствует скрытой позиции прайс листа. Необходимо указать активную позицию из текущего действующего прайс-листа..
        /// </summary>
        internal static string OrderPositionCorrespontToInactivePosition {
            get {
                return ResourceManager.GetString("OrderPositionCorrespontToInactivePosition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для позиции {0} в рубрику {1} отсутствует CPC.
        /// </summary>
        internal static string OrderPositionCostPerClickMustBeSpecified {
            get {
                return ResourceManager.GetString("OrderPositionCostPerClickMustBeSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для позиции {0} в рубрику {1} указан CPC меньше минимального.
        /// </summary>
        internal static string OrderPositionCostPerClickMustNotBeLessMinimum {
            get {
                return ResourceManager.GetString("OrderPositionCostPerClickMustNotBeLessMinimum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Позиция {0} не соответствует актуальному прайс-листу. Необходимо указать позицию из текущего действующего прайс-листа..
        /// </summary>
        internal static string OrderPositionMustCorrespontToActualPrice {
            get {
                return ResourceManager.GetString("OrderPositionMustCorrespontToActualPrice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Позиция &quot;{0}&quot; не может быть продана в рубрику &quot;{1}&quot; проекта &quot;{2}&quot; в выпуск {3:MMMM yyy}.
        /// </summary>
        internal static string OrderPositionSalesModelMustMatchCategorySalesModel {
            get {
                return ResourceManager.GetString("OrderPositionSalesModelMustMatchCategorySalesModel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Необходимо заполнить все обязательные для заполнения поля: {0}.
        /// </summary>
        internal static string OrderRequiredFieldsShouldBeSpecified {
            get {
                return ResourceManager.GetString("OrderRequiredFieldsShouldBeSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Отсутствует сканированная копия Бланка заказа.
        /// </summary>
        internal static string OrderScanShouldPresent {
            get {
                return ResourceManager.GetString("OrderScanShouldPresent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Заказ не содержит ни одной позиции.
        /// </summary>
        internal static string OrderShouldHaveAtLeastOnePosition {
            get {
                return ResourceManager.GetString("OrderShouldHaveAtLeastOnePosition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Договор не может иметь дату подписания позднее даты подписания заказа.
        /// </summary>
        internal static string OrderShouldNotBeSignedBeforeBargain {
            get {
                return ResourceManager.GetString("OrderShouldNotBeSignedBeforeBargain", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Адрес {0} принадлежит фирме-рекламодателю {1} с заказом, содержащим пакет &quot;Базовый&quot; или контекстный банер {2}.
        /// </summary>
        internal static string PartnerAdvertisementCouldNotCauseProblemsToTheAdvertiser {
            get {
                return ResourceManager.GetString("PartnerAdvertisementCouldNotCauseProblemsToTheAdvertiser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Адрес {0} принадлежит фирме-рекламодателю {1} с заказом {2}.
        /// </summary>
        internal static string PartnerAdvertisementShouldNotBeSoldToAdvertiser {
            get {
                return ResourceManager.GetString("PartnerAdvertisementShouldNotBeSoldToAdvertiser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Превышено допустимое количество POI на вход: {0}. Месяц: {1}. Адрес: {2}. Вход: {3}. Конфликтующие заказы: {4}.
        /// </summary>
        internal static string PoiLimitExceededForTheEntrance {
            get {
                return ResourceManager.GetString("PoiLimitExceededForTheEntrance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для рубрики {0} в проекте {1} в выпуск {2:MMMM yyy} не указан минимальный CPC.
        /// </summary>
        internal static string ProjectMustContainCostPerClickMinimumRestriction {
            get {
                return ResourceManager.GetString("ProjectMustContainCostPerClickMinimumRestriction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Подпозиция &quot;{0}&quot; позиции.
        /// </summary>
        internal static string RichChildPositionTypeTemplate {
            get {
                return ResourceManager.GetString("RichChildPositionTypeTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Позиция.
        /// </summary>
        internal static string RichDefaultPositionTypeTemplate {
            get {
                return ResourceManager.GetString("RichDefaultPositionTypeTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Тематика {0} использует удаленную рубрику {1}..
        /// </summary>
        internal static string ThemeCategoryMustBeActiveAndNotDeleted {
            get {
                return ResourceManager.GetString("ThemeCategoryMustBeActiveAndNotDeleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Заказ {0} не может иметь продаж в тематику {1}, поскольку тематика действует не весь период размещения заказа.
        /// </summary>
        internal static string ThemePeriodMustContainOrderPeriod {
            get {
                return ResourceManager.GetString("ThemePeriodMustContainOrderPeriod", resourceCulture);
            }
        }
    }
}
