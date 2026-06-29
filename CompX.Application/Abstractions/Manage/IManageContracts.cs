using CompX.Domain.Manage;
using CompX.SharedKernel.Contracts.Manage.Dtos;

namespace CompX.Application.Abstractions.Manage;

public interface IManageRepository
{
    Task<ManageSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManagePermissionDto>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);

    #region....Company.....
    Task<IReadOnlyList<CompanyDto>> GetAllCompanyAsync(CancellationToken cancellationToken = default);
    Task<bool> CompanyCodeExistsAsync(string companyCode, int? excludingCompanyId, CancellationToken cancellationToken = default);
    Task<int> CreateCompanyAsync(CompanyDto model, CancellationToken cancellationToken = default);
    Task<bool> UpdateCompanyAsync(CompanyDto model, CancellationToken cancellationToken = default);
    Task<bool> Company_DestroyAsync(int companyId, string modifiedBy, CancellationToken cancellationToken = default);
    #endregion

    #region....Nurse.....
    Task<IReadOnlyList<NurseDto>> GetAllNurseAsync(string? nurseFirstName, string? nurseLastName, string? comapnyName, int salesperson, CancellationToken cancellationToken = default);
    Task<int> CreateNurseAsync(NurseDto model, CancellationToken cancellationToken = default);
    Task<bool> UpdateNurseAsync(NurseDto model, CancellationToken cancellationToken = default);
    Task<bool> Nurse_DestroyAsync(int nurseId, string modifiedBy, CancellationToken cancellationToken = default);
    #endregion

    #region....Adjuster.....
    Task<IReadOnlyList<AdjusterDto>> GetAllAdjusterAsync(string? adjusterFirstName, string? adjusterLastName, int carrierParent, int salesperson, CancellationToken cancellationToken = default);
    Task<bool> AdjusterCodeExistsAsync(string adjusterCode, int? excludingAdjusterId, CancellationToken cancellationToken = default);
    Task<int> CreateAdjusterAsync(AdjusterDto model, CancellationToken cancellationToken = default);
    Task<bool> UpdateAdjusterAsync(AdjusterDto model, CancellationToken cancellationToken = default);
    Task<bool> Adjuster_DestroyAsync(int adjusterId, string modifiedBy, CancellationToken cancellationToken = default);
    #endregion

    #region....Carrier.....
    Task<IReadOnlyList<CarrierParentDto>> GetAllCarrierParentAsync(CancellationToken cancellationToken = default);
    Task<bool> ParentCodeExistsAsync(string parentCode, int? excludingCarrierParentId, CancellationToken cancellationToken = default);
    Task<bool> ChargeCodeExistsAsync(string chargeCode, int carrierParentId, string? jurisdiction, DateTime? effectiveFrom, decimal? rates, CancellationToken cancellationToken = default);
    Task<bool> ValidateChargeCodeAsync(string chargeCode, int carrierParentId, int department, CancellationToken cancellationToken = default);
    Task<int> CarrierParent_CreateAsync(CarrierParentDto model, CancellationToken cancellationToken = default);
    Task<bool> CarrierParent_UpdateAsync(CarrierParentDto model, CancellationToken cancellationToken = default);
    Task<bool> CarrierParent_DestroyAsync(int carrierParentId, string modifiedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierParentDto>> CarrierChild_ReadAsync(int parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierParentDto>> LoadCarrierChildDtailsAsync(int parentId, CancellationToken cancellationToken = default);
    Task<bool> ChildCodeExistsAsync(string childCode, int? excludingCarrierChildId, CancellationToken cancellationToken = default);
    Task<int> CarrierChild_CreateAsync(CarrierParentDto model, CancellationToken cancellationToken = default);
    Task<bool> CarrierChild_UpdateAsync(CarrierParentDto model, CancellationToken cancellationToken = default);
    Task<bool> CarrierChild_DestroyAsync(int carrierChildId, string modifiedBy, CancellationToken cancellationToken = default);
    Task<int> AddFeeScheduleAsync(FeeScheduleDto model, CancellationToken cancellationToken = default);
    Task<bool> DeleteFeeScheduleAsync(int feeScheduleId, string modifiedBy, CancellationToken cancellationToken = default);
    Task<bool> UpdateFeeScheduleAsync(FeeScheduleDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeScheduleDto>> GetFeeScheduleInfoAsync(int payerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeScheduleDto>> GetFeeScheduleInfoAsync(int payerId, string? chargeCode, int? department, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceTypeLookupDto>> GetServiceTypeOnManagePayerAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceTypeLookupDto>> GetServiceTypeOnProfitLossReportAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PointsToLookupDto>> GetPointsToAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<string?> GetServiceTypeNameAsync(short? serviceTypeId, CancellationToken cancellationToken = default);
    Task<string?> GetPointsToTypeNameAsync(decimal? pointsToId, CancellationToken cancellationToken = default);
    Task<long> AddChargeCodeDetailsAsync(CarrierParentDto model, CancellationToken cancellationToken = default);
    Task<bool> UpdateChargeCodeAsync(CarrierParentDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierParentDto>> GetChargeCodeInfoAsync(int payerId, CancellationToken cancellationToken = default);
    Task<bool> DeleteChargeCodeAsync(long chargeCodeId, string modifiedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChargeCodeLookupDto>> GetChargeCodeDetailsAsync(int? payerId, int departmentId, string? searchValue, CancellationToken cancellationToken = default);
    #endregion

    #region....Suplier......
    Task<IReadOnlyList<SupplierDto>> GetAllSupplierAsync(int? parentId, string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, string? Branchadress, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> GetAllSupplierBranchAsync(int? parentId, string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, string? Branchadress, int? providerStatus, CancellationToken cancellationToken = default);
    Task<int> GetSupplierActiveBranchCountAsync(int? supplierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDepartmentIDofSupplierAsync(int? supplierId, CancellationToken cancellationToken = default);
    Task<int> ProviderDuplicateOrNotAsync(string? providername, string? adress, string? zip, string? supplierid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> GetAllSuppliersAsync(string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, bool vendorQueue, int? providerStatus, string? SubSpecialty, string? Branchadress, string? ContractingEntity, string? SubSpecialtyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> GetAllSuppliersBranchesAsync(string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, bool vendorQueue, int? providerStatus, string? SubSpecialty, string? Branchadress, string? ContractingEntity, string? SubSpecialtyId, bool vendorBranchesShow, bool vendorContractingEntityShow, string? InternalProid, string? tin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> GetAllSuppliersCollapseAsync(int? ContractingEntityId, int SupplierId, int? ParentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> GetAllSuppliersParentCollapseAsync(int? ContractingEntityId, int SupplierId, int? ParentId, int? SupplierIdCollapse, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> GetAllSuppliersBranchCollapseAsync(int? ContractingEntityId, int SupplierId, int? ParentId, int? SupplierIdCollapse, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> GetSuppliersForSchedulingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllVendorAsync(string? searchValue, string? contractingEntityId, int providerStatus, int departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllAdjusterFirstNameAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllAdjusterLastNameAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllCountyAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllVendorAddressAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllBranchAddressAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllVendorCityAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllNurseFirstNameAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllNurseLastNameAsync(string? searchValue, CancellationToken cancellationToken = default);
    Task<bool> SaveDocumentsAsync(DocumentDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentDto>> GetVendorDocumentsAsync(int vendorId, CancellationToken cancellationToken = default);
    Task<bool> DeleteDocumentAsync(long documentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetVendorDocumentUploadTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProviderContractedRatesDto>> GetProviderRateaDetailAsync(int? supplierId, int? providerType, CancellationToken cancellationToken = default);
    Task<int> CommonBillingEnableDisableDetailsAsync(int? supplierId, int? providerType, CancellationToken cancellationToken = default);
    Task<bool> UpdateParentCommonBillingAsync(int? supplierId, int? providerType, string modifiedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetSubSpecialityNamesByIdsAsync(IReadOnlyList<int> subSpecialityIds, CancellationToken cancellationToken = default);
    Task<int> CreateNewSuppilerAsync(SupplierDto model, CancellationToken cancellationToken = default);
    Task<int> CreateNewSuppiler_CopyAsync(SupplierDto model, CancellationToken cancellationToken = default);
    Task UpdateSupplierAsync(SupplierDto model, CancellationToken cancellationToken = default);
    Task UpdateSupplier_CopyAsync(SupplierDto model, CancellationToken cancellationToken = default);
    Task DeleteSupplierAsync(SupplierDto model, CancellationToken cancellationToken = default);
    #endregion

    #region....ProviderContractedRates......
    Task<IReadOnlyList<ProviderContractedRatesDto>> GetProviderContractedRatesAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? internalProid, bool vendorContractingEntityShow, bool vendorParentShow, string? txtProvider, int? externalId, string? tinId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<int> SaveProviderContractedRatesAsync(ProviderContractedRatesDto model, CancellationToken cancellationToken = default);
    Task<bool> IsParentRatesExistAsync(ProviderContractedRatesDto model, CancellationToken cancellationToken = default);
    Task ProviderContractedRates_DestroyAsync(ProviderContractedRatesDto model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllSupplierIdAsync(string? parentSearchValue, string? branchSearchValue, string? contractingEntityId, int parentid, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierDto>> GetAllParenBranch_ReadAsync(bool? isParent, bool? isBranch, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProviderContractedRatesExportDto>> GetProviderContractedRatesExportAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? internalProid, bool vendorContractingEntityShow, bool vendorParentShow, string? txtProvider, int? externalId, string? tinId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProviderContractedRatesCompIQExportDto>> GetProviderContractedRatesCompiqExportAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? internalProid, bool vendorContractingEntityShow, bool vendorParentShow, string? txtProvider, int? externalId, string? tinId, DateTime? fromDate, DateTime? toDate, IReadOnlyList<ProviderContractedRatesDto>? selectedData, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetAllDiscountTypeAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PhysicianDto>> GetAllPhysicianAsync(CancellationToken cancellationToken = default);
    #endregion

    Task<IReadOnlyList<BillingRegionsDto>> GetAllBillingRegionsAsync(CancellationToken cancellationToken = default);
    Task<bool> IsBillingRegionExistsAsync(string regionName, int? excludingBillingRegionId, CancellationToken cancellationToken = default);
    Task CreateBillingRegionAsync(BillingRegionsDto model, CancellationToken cancellationToken = default);
    Task<bool> CptCodeExistsAsync(string cptCode, int? subSpecialityId, string? modifier, long? excludingCptCodeId, CancellationToken cancellationToken = default);
    Task<ManageRole?> GetRoleAsync(int roleId, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(int roleId, string roleName, bool isDeleted, string modifiedBy, CancellationToken cancellationToken = default);

    Task<CompanyProfile?> GetCompanyAsync(int companyId, CancellationToken cancellationToken = default);
    Task UpdateCompanyAsync(int companyId, string companyCode, bool isDeleted, string modifiedBy, CancellationToken cancellationToken = default);

    Task<CptCodeRule?> GetCptCodeAsync(long cptCodeId, CancellationToken cancellationToken = default);
    Task UpdateCptCodeAsync(long cptCodeId, string cptCode, int? subSpecialityId, string? modifier, bool isDeleted, string modifiedBy, CancellationToken cancellationToken = default);
}
