using CompX.SharedKernel.Contracts.Manage.Dtos;
using CompX.SharedKernel.Contracts.Manage.Requests;
using CompX.SharedKernel.Contracts.Manage.Responses;

namespace CompX.Application.Abstractions.Services;

public interface IManageApplicationService
{
    #region....Company.....
    Task<bool> CompanyAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CompanyDto>> Company_ReadAsync(CancellationToken cancellationToken = default);
    Task<bool> CompanyCodeExistsAsync(string companyCode, CancellationToken cancellationToken = default);
    Task<CompanyDto> Company_CreateAsync(CompanyDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<string> AddCompany_CreateAsync(CompanyDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<CompanyDto?> Company_UpdateAsync(CompanyDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<bool> Company_DestroyAsync(CompanyDto model, string modifiedBy, CancellationToken cancellationToken = default);
    #endregion

    #region....Nurse.....
    Task<bool> NurseAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NurseDto>> Nurse_ReadAsync(string? nurseFirstName, string? nurseLastName, string? comapnyName, int salesperson, CancellationToken cancellationToken = default);
    Task<NurseDto> Nurse_CreateAsync(NurseDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<NurseDto?> Nurse_UpdateAsync(NurseDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<bool> GetCompanyEditorAsync(CancellationToken cancellationToken = default);
    Task<bool> Nurse_DestroyAsync(NurseDto model, string modifiedBy, CancellationToken cancellationToken = default);
    #endregion

    #region....Adjuster.....
    Task<bool> AdjusterAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdjusterDto>> Adjuster_ReadAsync(string? adjusterFirstName, string? adjusterLastName, int carrierParent, int salesperson, CancellationToken cancellationToken = default);
    Task<bool> AdjusterCodeExistsAsync(string adjusterCode, CancellationToken cancellationToken = default);
    Task<AdjusterDto> Adjuster_CreateAsync(AdjusterDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<AdjusterDto?> Adjuster_UpdateAsync(AdjusterDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<bool> Adjuster_DestroyAsync(AdjusterDto model, string modifiedBy, CancellationToken cancellationToken = default);
    #endregion

    #region....Carrier.....
    Task<bool> CarrierAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierParentDto>> CarrierParent_ReadAsync(CancellationToken cancellationToken = default);
    Task<bool> ParentCodeExistsAsync(string parentCode, CancellationToken cancellationToken = default);
    Task<bool> ChargeCodeExistsAsync(string chargeCode, int carrierParentId, string? jurisdiction, DateTime? effectiveFrom, decimal? rates, CancellationToken cancellationToken = default);
    Task<bool> ValidateChargeCodeAsync(string chargeCode, int carrierParentId, int department, CancellationToken cancellationToken = default);
    Task<CarrierParentDto> CarrierParent_CreateAsync(CarrierParentDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<CarrierParentDto?> CarrierParent_UpdateAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<bool> CarrierParent_DestroyAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierParentDto>> CarrierChild_ReadAsync(int parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierParentDto>> LoadCarrierChildDtailsAsync(int parentId, CancellationToken cancellationToken = default);
    Task<bool> ChildCodeExistsAsync(string childCode, CancellationToken cancellationToken = default);
    Task<CarrierParentDto> CarrierChild_CreateAsync(CarrierParentDto model, int? id, string createdBy, CancellationToken cancellationToken = default);
    Task<CarrierParentDto> AddCarrierChild_CreateAsync(CarrierParentDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<CarrierParentDto?> CarrierChild_UpdateAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<bool> CarrierChild_DestroyAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PayersContractRateDto>> AddGridViewValueAsync(PayersContractRateDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeScheduleDto>> AddFeeScheduleValueAsync(FeeScheduleDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<object> FeeSchedule_DestroyAsync(FeeScheduleDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<object> FeeSchedule_UpdateAsync(FeeScheduleDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeScheduleDto>> FeeSchedule_ReadAsync(int? payerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeScheduleDto>> ChargeCodeHistory_ReadAsync(int? payerId, string? chargeCode, int? department, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceTypeLookupDto>> ServiceType_ReadAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceTypeLookupDto>> AllServiceTypes_ReadAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PointsToLookupDto>> PointsTo_ReadAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierParentDto>> AddChargeCodeDetailsAsync(CarrierParentDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<object> ChargeCode_UpdateAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierParentDto>> ChargeCodeDetails_ReadAsync(int? payerId, CancellationToken cancellationToken = default);
    Task<object> ChargeCode_DestroyAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChargeCodeLookupDto>> GetChargeCodeDetailsAsync(int? payerId, int departmentId, string? searchValue, CancellationToken cancellationToken = default);
    #endregion

    #region....Suplier......
    Task<bool> SupplierAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> Supplier_ReadAsync(int? parentId, string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, string? Branchadress, CancellationToken cancellationToken = default);
    Task<int> GetSupplierBranchCountAsync(int? parentId, string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, string? Branchadress, CancellationToken cancellationToken = default);
    Task<int> GetAllSupplierBranchCountAsync(int? parentId, string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, string? Branchadress, int? providerStatus, CancellationToken cancellationToken = default);
    Task<int> GetSupplierActiveBranchCountAsync(int? SupplierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDepartmentIDofSupplierAsync(int? SupplierId, CancellationToken cancellationToken = default);
    Task<int> ProviderDuplicateOrNotAsync(string? providername, string? adress, string? zip, string? supplierid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> ParentSupplier_ReadAsync(string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, bool vendorQueue, int? providerStatus, string? SubSpecialty, string? Branchadress, string? ContractingEntity, string? SubSpecialtyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> ParentSupplierBranch_ReadAsync(string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, bool vendorQueue, int? providerStatus, string? SubSpecialty, string? Branchadress, string? ContractingEntity, string? SubSpecialtyId, bool vendorBranchesShow, bool vendorContractingEntityShow, string? InternalProid, string? tin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> ContractingEntitySupplierCollapse_ReadAsync(int? ContractingEntityId, int SupplierId, int? ParentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> ParentSupplierCollapse_ReadAsync(int? ContractingEntityId, int SupplierId, int? ParentId, int? SupplierIdCollapse, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> BranchSupplierCollapse_ReadAsync(int? ContractingEntityId, int SupplierId, int? ParentId, int? SupplierIdCollapse, CancellationToken cancellationToken = default);
    Task<SupplierDto> Supplier_CreateAsync(SupplierDto model, string createdBy, string? pageName, CancellationToken cancellationToken = default);
    Task<SupplierDto> Supplier_Create_CopyAsync(SupplierDto model, string createdBy, string? pageName, CancellationToken cancellationToken = default);
    Task<SupplierDto> Supplier_UpdateAsync(SupplierDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<SupplierDto> Supplier_Update_CopyAsync(SupplierDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task<SupplierDto> Supplier_DestroyAsync(SupplierDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> Supplier_GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageStatusLookupDto>> VendorStatus_ReadAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageStatusLookupDto>> ProviderStatusReadAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageStatusLookupDto>> ProviderStatusReaderAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> Vendor_ReadAsync(string? searchValue, string? contractingEntityId, int providerStatus, int departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> AdjusterFirstName_ReadAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> AdjusterLastName_ReadAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> County_ReadAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> VendorAddress_ReadAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> BranchAddress_ReadAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> VendorCity_ReadAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> NurseFirstName_ReadAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> NurseLastName_ReadAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<bool> VendorDocument_SaveAsync(DocumentDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentDto>> VendorDocuments_ReadAsync(int? vendorId, CancellationToken cancellationToken = default);
    Task<bool> VendorDocument_DestroyAsync(DocumentDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> VendorDocumentUploadTypes_ReadAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProviderContractedRatesDto>> ProviderRatesStatus_ReadAsync(int? supplierId, int? providerType, CancellationToken cancellationToken = default);
    Task<int> CommonBillingEnableDisable_ReadAsync(int? supplierId, int? providerType, CancellationToken cancellationToken = default);
    Task<bool> UpdateParentCommonBillingAsync(int? supplierId, int? providerType, string modifiedBy, CancellationToken cancellationToken = default);
    #endregion

    #region....ProviderContractedRates......
    Task<IReadOnlyList<ProviderContractedRatesDto>> ProviderContractedRates_ReadAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? InternalProid, bool VendorContractingEntityShow, bool VendorParentShow, string? txtProvider, int? ExternalId, string? TinId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<int> ProviderContractedRates_CreateAsync(ProviderContractedRatesDto model, string createdBy, CancellationToken cancellationToken = default);
    Task<bool> ProviderParent_ExistsAsync(ProviderContractedRatesDto model, CancellationToken cancellationToken = default);
    Task<int> ProviderContractedRates_updateAsync(ProviderContractedRatesDto model, string modifiedBy, CancellationToken cancellationToken = default);
    Task ProviderContractedRates_DestroyAsync(ProviderContractedRatesDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> AllSupplier_ReadAsync(string? parentSearchValue, string? branchSearchValue, string? contractingEntityId, int parentid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> AllParenBranch_ReadAsync(bool? IsParent, bool? IsBranch, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProviderContractedRatesExportDto>> ProviderRatesExportExcelAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? InternalProid, bool VendorContractingEntityShow, bool VendorParentShow, string? txtProvider, int? ExternalId, string? TinId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProviderContractedRatesCompIQExportDto>> ProviderRatesCompIQExportAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? InternalProid, bool VendorContractingEntityShow, bool VendorParentShow, string? txtProvider, int? ExternalId, string? TinId, DateTime? fromDate, DateTime? toDate, IReadOnlyList<ProviderContractedRatesDto>? selectedData, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> DiscountType_ReadAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProviderContractedRatesCompIQExportDto>> GenerateCIQUploadExcelAsync(IReadOnlyList<ProviderContractedRatesDto> selectedData, CancellationToken cancellationToken = default);
    Task<bool> ExportProviderRatesCompiqExcelAndUploadAsync(IReadOnlyList<ProviderContractedRatesCompIQExportDto> data, CancellationToken cancellationToken = default);
    Task<bool> PhysicianAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PhysicianDto>> Physician_ReadAsync(CancellationToken cancellationToken = default);
    #endregion

    Task<ManageSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManagePermissionDto>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BillingRegionsDto>> BillingRegions_ReadAsync(CancellationToken cancellationToken = default);
    Task<bool> IsBillingRegionExistsAsync(string regionName, CancellationToken cancellationToken = default);
    Task BillingRegions_CreateAsync(CreateBillingRegionRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpsertRoleAsync(int roleId, UpsertRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpsertCompanyAsync(int companyId, UpsertCompanyRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpsertCptCodeAsync(long cptCodeId, UpsertCptCodeRequest request, CancellationToken cancellationToken = default);
    Task<ManageParityPreviewResponse> PreviewAsync(ManageParityPreviewRequest request, CancellationToken cancellationToken = default);
}
