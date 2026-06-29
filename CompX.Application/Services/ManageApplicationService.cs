using CompX.Application.Abstractions.Manage;
using CompX.Application.Abstractions.Persistence;
using CompX.Application.Abstractions.Services;
using CompX.Domain.Manage;
using CompX.SharedKernel.Contracts.Manage.Dtos;
using CompX.SharedKernel.Contracts.Manage.Requests;
using CompX.SharedKernel.Contracts.Manage.Responses;

namespace CompX.Application.Services;

public sealed class ManageApplicationService : IManageApplicationService
{
    private readonly IManageRepository manageRepo;
    private readonly IUnitOfWork uow;
    private static readonly List<PayersContractRateDto> contractRateList = [];
    private static readonly List<FeeScheduleDto> feeScheduleList = [];
    private static readonly List<CarrierParentDto> chargeCodeList = [];
    private static readonly List<ProviderContractedRatesCompIQExportDto> compIqUploadData = [];

    public ManageApplicationService(IManageRepository manageRepo, IUnitOfWork uow)
    {
        this.manageRepo = manageRepo;
        this.uow = uow;
    }

    public Task<ManageSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default) => manageRepo.GetSummaryAsync(cancellationToken);

    #region....Company.....

    public Task<bool> CompanyAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<IReadOnlyList<CompanyDto>> Company_ReadAsync(CancellationToken cancellationToken = default)
        => manageRepo.GetAllCompanyAsync(cancellationToken);

    public Task<bool> CompanyCodeExistsAsync(string companyCode, CancellationToken cancellationToken = default)
        => manageRepo.CompanyCodeExistsAsync(companyCode, excludingCompanyId: null, cancellationToken);

    public async Task<CompanyDto> Company_CreateAsync(CompanyDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        if (await manageRepo.CompanyCodeExistsAsync(model.CompanyCode ?? string.Empty, null, cancellationToken))
        {
            return model;
        }

        var insertModel = model with
        {
            CompanyId = 0,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            Status = 1
        };

        var id = await manageRepo.CreateCompanyAsync(insertModel, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return insertModel with { CompanyId = id };
    }

    public async Task<string> AddCompany_CreateAsync(CompanyDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        var created = await Company_CreateAsync(model, createdBy, cancellationToken);
        return created.CompanyId > 0 ? created.CompanyId.ToString() : string.Empty;
    }

    public async Task<CompanyDto?> Company_UpdateAsync(CompanyDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var updateModel = model with
        {
            ModifiedBy = modifiedBy,
            ModifiedDate = DateTime.UtcNow
        };
        var updated = await manageRepo.UpdateCompanyAsync(updateModel, cancellationToken);
        if (!updated) return null;
        await uow.SaveChangesAsync(cancellationToken);
        return updateModel;
    }

    public async Task<bool> Company_DestroyAsync(CompanyDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var deleted = await manageRepo.Company_DestroyAsync(model.CompanyId, modifiedBy, cancellationToken);
        if (!deleted) return false;
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    #endregion

    #region....Nurse.....

    public Task<bool> NurseAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<IReadOnlyList<NurseDto>> Nurse_ReadAsync(string? nurseFirstName, string? nurseLastName, string? comapnyName, int salesperson, CancellationToken cancellationToken = default)
        => manageRepo.GetAllNurseAsync(nurseFirstName, nurseLastName, comapnyName, salesperson, cancellationToken);

    public async Task<NurseDto> Nurse_CreateAsync(NurseDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        var insertModel = model with
        {
            NurseId = 0,
            Phone = SanitizePhone(model.Phone),
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            Status = 1
        };
        var nurseId = await manageRepo.CreateNurseAsync(insertModel, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return insertModel with { NurseId = nurseId };
    }

    public async Task<NurseDto?> Nurse_UpdateAsync(NurseDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var updateModel = model with
        {
            Phone = SanitizePhone(model.Phone),
            ModifiedBy = modifiedBy,
            ModifiedDate = DateTime.UtcNow
        };
        var updated = await manageRepo.UpdateNurseAsync(updateModel, cancellationToken);
        if (!updated) return null;
        await uow.SaveChangesAsync(cancellationToken);
        return updateModel;
    }

    public Task<bool> GetCompanyEditorAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public async Task<bool> Nurse_DestroyAsync(NurseDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var deleted = await manageRepo.Nurse_DestroyAsync(model.NurseId, modifiedBy, cancellationToken);
        if (!deleted) return false;
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    #endregion

    #region....Adjuster.....

    public Task<bool> AdjusterAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<IReadOnlyList<AdjusterDto>> Adjuster_ReadAsync(string? adjusterFirstName, string? adjusterLastName, int carrierParent, int salesperson, CancellationToken cancellationToken = default)
        => manageRepo.GetAllAdjusterAsync(adjusterFirstName, adjusterLastName, carrierParent, salesperson, cancellationToken);

    public Task<bool> AdjusterCodeExistsAsync(string adjusterCode, CancellationToken cancellationToken = default)
        => manageRepo.AdjusterCodeExistsAsync(adjusterCode, excludingAdjusterId: null, cancellationToken);

    public async Task<AdjusterDto> Adjuster_CreateAsync(AdjusterDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        if (await manageRepo.AdjusterCodeExistsAsync(model.AdjusterCode ?? string.Empty, null, cancellationToken))
        {
            return model;
        }

        var insertModel = model with
        {
            AdjusterId = 0,
            Phone = SanitizePhone(model.Phone),
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            Status = 1
        };

        var adjusterId = await manageRepo.CreateAdjusterAsync(insertModel, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return insertModel with { AdjusterId = adjusterId };
    }

    public async Task<AdjusterDto?> Adjuster_UpdateAsync(AdjusterDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var updateModel = model with
        {
            Phone = SanitizePhone(model.Phone),
            ModifiedBy = modifiedBy,
            ModifiedDate = DateTime.UtcNow
        };
        var updated = await manageRepo.UpdateAdjusterAsync(updateModel, cancellationToken);
        if (!updated) return null;
        await uow.SaveChangesAsync(cancellationToken);
        return updateModel;
    }

    public async Task<bool> Adjuster_DestroyAsync(AdjusterDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var deleted = await manageRepo.Adjuster_DestroyAsync(model.AdjusterId, modifiedBy, cancellationToken);
        if (!deleted) return false;
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    #endregion

    #region....Carrier.....

    public Task<bool> CarrierAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<IReadOnlyList<CarrierParentDto>> CarrierParent_ReadAsync(CancellationToken cancellationToken = default)
        => manageRepo.GetAllCarrierParentAsync(cancellationToken);

    public Task<bool> ParentCodeExistsAsync(string parentCode, CancellationToken cancellationToken = default)
        => manageRepo.ParentCodeExistsAsync(parentCode, excludingCarrierParentId: null, cancellationToken);

    public Task<bool> ChargeCodeExistsAsync(string chargeCode, int carrierParentId, string? jurisdiction, DateTime? effectiveFrom, decimal? rates, CancellationToken cancellationToken = default)
        => manageRepo.ChargeCodeExistsAsync(chargeCode, carrierParentId, jurisdiction, effectiveFrom, rates, cancellationToken);

    public Task<bool> ValidateChargeCodeAsync(string chargeCode, int carrierParentId, int department, CancellationToken cancellationToken = default)
        => manageRepo.ValidateChargeCodeAsync(chargeCode, carrierParentId, department, cancellationToken);

    public async Task<CarrierParentDto> CarrierParent_CreateAsync(CarrierParentDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(model.ParentCode) && await manageRepo.ParentCodeExistsAsync(model.ParentCode, null, cancellationToken))
        {
            return model;
        }

        model.Status = true;
        model.CreatedBy = createdBy;
        model.CreatedDate = DateTime.UtcNow;

        var carrierId = await manageRepo.CarrierParent_CreateAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        model.CarrierId = carrierId;
        model.CarrierParentId = carrierId;
        return model;
    }

    public async Task<CarrierParentDto?> CarrierParent_UpdateAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        model.ModifiedBy = modifiedBy;
        model.ModifiedDate = DateTime.UtcNow;
        var updated = await manageRepo.CarrierParent_UpdateAsync(model, cancellationToken);
        if (!updated) return null;
        await uow.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<bool> CarrierParent_DestroyAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var deleted = await manageRepo.CarrierParent_DestroyAsync(model.CarrierParentId ?? model.CarrierId ?? 0, modifiedBy, cancellationToken);
        if (!deleted) return false;
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<IReadOnlyList<CarrierParentDto>> CarrierChild_ReadAsync(int parentId, CancellationToken cancellationToken = default)
        => manageRepo.CarrierChild_ReadAsync(parentId, cancellationToken);

    public Task<IReadOnlyList<CarrierParentDto>> LoadCarrierChildDtailsAsync(int parentId, CancellationToken cancellationToken = default)
        => manageRepo.LoadCarrierChildDtailsAsync(parentId, cancellationToken);

    public Task<bool> ChildCodeExistsAsync(string childCode, CancellationToken cancellationToken = default)
        => manageRepo.ChildCodeExistsAsync(childCode, excludingCarrierChildId: null, cancellationToken);

    public async Task<CarrierParentDto> CarrierChild_CreateAsync(CarrierParentDto model, int? id, string createdBy, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(model.ChildCode) && await manageRepo.ChildCodeExistsAsync(model.ChildCode, null, cancellationToken))
        {
            return model;
        }

        model.ParentId = id;
        model.Status = true;
        model.CreatedBy = createdBy;
        model.CreatedDate = DateTime.UtcNow;

        var carrierChildId = await manageRepo.CarrierChild_CreateAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        model.CarrierChildId = carrierChildId;
        return model;
    }

    public async Task<CarrierParentDto> AddCarrierChild_CreateAsync(CarrierParentDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(model.ChildCode) && await manageRepo.ChildCodeExistsAsync(model.ChildCode, null, cancellationToken))
        {
            return model;
        }

        if (model.ParentId is null && int.TryParse(model.ParentCode, out var parentId))
        {
            model.ParentId = parentId;
        }

        model.Status = true;
        model.CreatedBy = createdBy;
        model.CreatedDate = DateTime.UtcNow;
        var carrierChildId = await manageRepo.CarrierChild_CreateAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        model.CarrierChildId = carrierChildId;
        return model;
    }

    public async Task<CarrierParentDto?> CarrierChild_UpdateAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        model.ModifiedBy = modifiedBy;
        model.ModifiedDate = DateTime.UtcNow;
        var updated = await manageRepo.CarrierChild_UpdateAsync(model, cancellationToken);
        if (!updated) return null;
        await uow.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<bool> CarrierChild_DestroyAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var carrierChildId = model.CarrierChildId ?? model.CarrierParentId ?? model.CarrierId ?? 0;
        var deleted = await manageRepo.CarrierChild_DestroyAsync(carrierChildId, modifiedBy, cancellationToken);
        if (!deleted) return false;
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<IReadOnlyList<PayersContractRateDto>> AddGridViewValueAsync(PayersContractRateDto model, CancellationToken cancellationToken = default)
    {
        var addContractRateInfo = model with { Id = contractRateList.Count };
        contractRateList.Add(addContractRateInfo);
        return Task.FromResult<IReadOnlyList<PayersContractRateDto>>(contractRateList.ToList());
    }

    public async Task<IReadOnlyList<FeeScheduleDto>> AddFeeScheduleValueAsync(FeeScheduleDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        var exists = await manageRepo.ChargeCodeExistsAsync(model.ChargeCode, model.PayerId ?? 0, model.Jurisdiction, model.EffectiveFrom, model.Rate, cancellationToken);
        if (!exists)
        {
            var feeScheduleDetail = model with
            {
                Departments = GetDepartmentType(model.Department ?? 0),
                PayerId = model.PayerId ?? 0,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,
                FeeScheduleId = -1 - feeScheduleList.Count
            };
            feeScheduleList.Add(feeScheduleDetail);

            if ((model.PayerId ?? 0) > 0)
            {
                await manageRepo.AddFeeScheduleAsync(feeScheduleDetail, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
            }
        }

        return feeScheduleList.Where(s => !s.IsDeleted).ToList();
    }

    public async Task<object> FeeSchedule_DestroyAsync(FeeScheduleDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        if ((model.PayerId ?? 0) > 0)
        {
            var deleted = await manageRepo.DeleteFeeScheduleAsync(model.FeeScheduleId, modifiedBy, cancellationToken);
            if (deleted)
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            return model with { IsDeleted = true, ModifiedBy = modifiedBy, ModifiedDate = DateTime.UtcNow };
        }

        var index = feeScheduleList.FindIndex(x => x.FeeScheduleId == model.FeeScheduleId);
        if (index >= 0)
        {
            feeScheduleList[index] = feeScheduleList[index] with { IsDeleted = true };
        }
        return feeScheduleList.Where(s => !s.IsDeleted).ToList();
    }

    public async Task<object> FeeSchedule_UpdateAsync(FeeScheduleDto model, CancellationToken cancellationToken = default)
    {
        if (model.FeeScheduleId < 0)
        {
            var index = feeScheduleList.FindIndex(x => x.FeeScheduleId == model.FeeScheduleId);
            if (index >= 0)
            {
                var current = feeScheduleList[index];
                feeScheduleList[index] = current with
                {
                    ChargeCode = model.ChargeCode,
                    ChargeDescription = model.ChargeDescription,
                    CPTCode = model.CPTCode,
                    Pos = model.Pos,
                    Department = model.Department,
                    Departments = GetDepartmentType(model.Department ?? 0),
                    Rate = model.NewRate,
                    IsDeleted = model.IsDeleted,
                    Jurisdiction = model.Jurisdiction
                };
            }
            return feeScheduleList.Where(s => !s.IsDeleted).ToList();
        }

        var updated = await manageRepo.UpdateFeeScheduleAsync(model, cancellationToken);
        if (updated)
        {
            await uow.SaveChangesAsync(cancellationToken);
        }
        return model;
    }

    public async Task<IReadOnlyList<FeeScheduleDto>> FeeSchedule_ReadAsync(int? payerId, CancellationToken cancellationToken = default)
    {
        var result = new List<FeeScheduleDto>();
        if ((payerId ?? 0) > 0)
        {
            feeScheduleList.Clear();
            result = (await manageRepo.GetFeeScheduleInfoAsync(payerId ?? 0, cancellationToken)).ToList();
        }

        result = result
            .Union(feeScheduleList.Where(s => s.PayerId == (payerId ?? 0) && !s.IsDeleted))
            .OrderByDescending(x => x.EffectiveFrom)
            .ToList();
        return result;
    }

    public async Task<IReadOnlyList<FeeScheduleDto>> ChargeCodeHistory_ReadAsync(int? payerId, string? chargeCode, int? department, CancellationToken cancellationToken = default)
    {
        var result = new List<FeeScheduleDto>();
        if ((payerId ?? 0) > 0)
        {
            feeScheduleList.Clear();
            result = (await manageRepo.GetFeeScheduleInfoAsync(payerId ?? 0, chargeCode, department, cancellationToken)).ToList();
        }

        result = result
            .Union(feeScheduleList.Where(s => s.PayerId == (payerId ?? 0) && !s.IsDeleted))
            .OrderByDescending(x => x.EffectiveFrom)
            .ToList();
        return result;
    }

    public Task<IReadOnlyList<ServiceTypeLookupDto>> ServiceType_ReadAsync(int departmentId, CancellationToken cancellationToken = default)
        => manageRepo.GetServiceTypeOnManagePayerAsync(departmentId, cancellationToken);

    public Task<IReadOnlyList<ServiceTypeLookupDto>> AllServiceTypes_ReadAsync(int departmentId, CancellationToken cancellationToken = default)
        => manageRepo.GetServiceTypeOnProfitLossReportAsync(departmentId, cancellationToken);

    public Task<IReadOnlyList<PointsToLookupDto>> PointsTo_ReadAsync(int departmentId, CancellationToken cancellationToken = default)
        => manageRepo.GetPointsToAsync(departmentId, cancellationToken);

    public async Task<IReadOnlyList<CarrierParentDto>> AddChargeCodeDetailsAsync(CarrierParentDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        var chargeCodeDetail = new CarrierParentDto
        {
            CarrierParentId = model.CarrierParentId,
            Department = model.Department,
            Departments = GetDepartmentType(model.Department ?? 0),
            ServiceType = model.ServiceType,
            ServiceTypeDescription = await manageRepo.GetServiceTypeNameAsync(model.ServiceType, cancellationToken),
            ChargeCode = model.ChargeCode,
            PointsTo = model.PointsTo,
            PointToTypes = await manageRepo.GetPointsToTypeNameAsync(model.PointsTo, cancellationToken),
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            ChargeCodeId = -1 - chargeCodeList.Count
        };

        if ((model.CarrierParentId ?? 0) > 0)
        {
            await manageRepo.AddChargeCodeDetailsAsync(chargeCodeDetail, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }
        else
        {
            chargeCodeList.Add(chargeCodeDetail);
        }

        return chargeCodeList.Where(s => s.IsDeleted != true).ToList();
    }

    public async Task<object> ChargeCode_UpdateAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        if ((model.ChargeCodeId ?? 0) < 0)
        {
            var index = chargeCodeList.FindIndex(x => x.ChargeCodeId == model.ChargeCodeId);
            if (index >= 0)
            {
                var item = chargeCodeList[index];
                item.Department = model.Department;
                item.Departments = GetDepartmentType(model.Department ?? 0);
                item.ChargeCode = model.ChargeCode;
                item.ServiceType = model.ServiceType;
                item.ServiceTypeDescription = await manageRepo.GetServiceTypeNameAsync(model.ServiceType, cancellationToken);
                item.PointsTo = model.PointsTo;
                item.PointToTypes = await manageRepo.GetPointsToTypeNameAsync(model.PointsTo, cancellationToken);
                item.IsDeleted = model.IsDeleted;
                chargeCodeList[index] = item;
            }

            return chargeCodeList.Where(s => s.IsDeleted != true).ToList();
        }

        model.ModifiedBy = modifiedBy;
        model.ModifiedDate = DateTime.UtcNow;
        var updated = await manageRepo.UpdateChargeCodeAsync(model, cancellationToken);
        if (updated)
        {
            await uow.SaveChangesAsync(cancellationToken);
        }
        return model;
    }

    public async Task<IReadOnlyList<CarrierParentDto>> ChargeCodeDetails_ReadAsync(int? payerId, CancellationToken cancellationToken = default)
    {
        var result = new List<CarrierParentDto>();
        if ((payerId ?? 0) > 0)
        {
            chargeCodeList.Clear();
            result = (await manageRepo.GetChargeCodeInfoAsync(payerId ?? 0, cancellationToken)).ToList();
        }

        result = result
            .Union(chargeCodeList.Where(s => s.CarrierParentId == payerId && s.IsDeleted != true))
            .ToList();

        return result;
    }

    public async Task<object> ChargeCode_DestroyAsync(CarrierParentDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        if ((model.CarrierParentId ?? 0) > 0)
        {
            var deleted = await manageRepo.DeleteChargeCodeAsync(model.ChargeCodeId ?? 0, modifiedBy, cancellationToken);
            if (deleted)
            {
                await uow.SaveChangesAsync(cancellationToken);
            }

            model.IsDeleted = true;
            return model;
        }

        var index = chargeCodeList.FindIndex(x => x.ChargeCodeId == model.ChargeCodeId);
        if (index >= 0)
        {
            var item = chargeCodeList[index];
            item.IsDeleted = true;
            chargeCodeList[index] = item;
        }

        return chargeCodeList.Where(s => s.IsDeleted != true).ToList();
    }

    public Task<IReadOnlyList<ChargeCodeLookupDto>> GetChargeCodeDetailsAsync(int? payerId, int departmentId, string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetChargeCodeDetailsAsync(payerId, departmentId, searchValue, cancellationToken);

    #endregion

    #region....Suplier......

    public Task<bool> SupplierAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<IReadOnlyList<SupplierDto>> Supplier_ReadAsync(int? parentId, string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, string? Branchadress, CancellationToken cancellationToken = default)
        => manageRepo.GetAllSupplierAsync(parentId, vendorName, vendorAddress, department, zip, city, state, county, fax, vendorStatus, radius, Branchadress, cancellationToken);

    public async Task<int> GetSupplierBranchCountAsync(int? parentId, string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, string? Branchadress, CancellationToken cancellationToken = default)
    {
        if ((parentId ?? 0) <= 0) return 0;
        var result = await manageRepo.GetAllSupplierAsync(parentId, vendorName, vendorAddress, department, zip, city, state, county, fax, vendorStatus, radius, Branchadress, cancellationToken);
        return result.Count;
    }

    public async Task<int> GetAllSupplierBranchCountAsync(int? parentId, string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, string? Branchadress, int? providerStatus, CancellationToken cancellationToken = default)
    {
        if ((parentId ?? 0) <= 0) return 0;
        var result = await manageRepo.GetAllSupplierBranchAsync(parentId, vendorName, vendorAddress, department, zip, city, state, county, fax, vendorStatus, radius, Branchadress, providerStatus, cancellationToken);
        return result.Count;
    }

    public Task<int> GetSupplierActiveBranchCountAsync(int? SupplierId, CancellationToken cancellationToken = default)
        => manageRepo.GetSupplierActiveBranchCountAsync(SupplierId, cancellationToken);

    public Task<IReadOnlyList<string>> GetDepartmentIDofSupplierAsync(int? SupplierId, CancellationToken cancellationToken = default)
        => manageRepo.GetDepartmentIDofSupplierAsync(SupplierId, cancellationToken);

    public Task<int> ProviderDuplicateOrNotAsync(string? providername, string? adress, string? zip, string? supplierid, CancellationToken cancellationToken = default)
        => manageRepo.ProviderDuplicateOrNotAsync(providername, adress, zip, supplierid, cancellationToken);

    public Task<IReadOnlyList<SupplierDto>> ParentSupplier_ReadAsync(string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, bool vendorQueue, int? providerStatus, string? SubSpecialty, string? Branchadress, string? ContractingEntity, string? SubSpecialtyId, CancellationToken cancellationToken = default)
        => manageRepo.GetAllSuppliersAsync(vendorName, vendorAddress, department, zip, city, state, county, fax, vendorStatus, radius, vendorQueue, providerStatus, SubSpecialty, Branchadress, ContractingEntity, SubSpecialtyId, cancellationToken);

    public Task<IReadOnlyList<SupplierDto>> ParentSupplierBranch_ReadAsync(string? vendorName, string? vendorAddress, string? department, string? zip, string? city, string? state, string? county, string? fax, int? vendorStatus, int? radius, bool vendorQueue, int? providerStatus, string? SubSpecialty, string? Branchadress, string? ContractingEntity, string? SubSpecialtyId, bool vendorBranchesShow, bool vendorContractingEntityShow, string? InternalProid, string? tin, CancellationToken cancellationToken = default)
        => manageRepo.GetAllSuppliersBranchesAsync(vendorName, vendorAddress, department, zip, city, state, county, fax, vendorStatus, radius, vendorQueue, providerStatus, SubSpecialty, Branchadress, ContractingEntity, SubSpecialtyId, vendorBranchesShow, vendorContractingEntityShow, InternalProid, tin, cancellationToken);

    public Task<IReadOnlyList<SupplierDto>> ContractingEntitySupplierCollapse_ReadAsync(int? ContractingEntityId, int SupplierId, int? ParentId, CancellationToken cancellationToken = default)
        => manageRepo.GetAllSuppliersCollapseAsync(ContractingEntityId, SupplierId, ParentId, cancellationToken);

    public Task<IReadOnlyList<SupplierDto>> ParentSupplierCollapse_ReadAsync(int? ContractingEntityId, int SupplierId, int? ParentId, int? SupplierIdCollapse, CancellationToken cancellationToken = default)
        => manageRepo.GetAllSuppliersParentCollapseAsync(ContractingEntityId, SupplierId, ParentId, SupplierIdCollapse, cancellationToken);

    public Task<IReadOnlyList<SupplierDto>> BranchSupplierCollapse_ReadAsync(int? ContractingEntityId, int SupplierId, int? ParentId, int? SupplierIdCollapse, CancellationToken cancellationToken = default)
        => manageRepo.GetAllSuppliersBranchCollapseAsync(ContractingEntityId, SupplierId, ParentId, SupplierIdCollapse, cancellationToken);

    public async Task<SupplierDto> Supplier_CreateAsync(SupplierDto model, string createdBy, string? pageName, CancellationToken cancellationToken = default)
    {
        await PrepareSupplierForUpsertAsync(model, cancellationToken);
        model.IsDeleted = false;
        model.CreatedBy = createdBy;
        model.PageName = pageName;
        model.SupplierId = await manageRepo.CreateNewSuppilerAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<SupplierDto> Supplier_Create_CopyAsync(SupplierDto model, string createdBy, string? pageName, CancellationToken cancellationToken = default)
    {
        await PrepareSupplierForUpsertAsync(model, cancellationToken);
        model.IsDeleted = false;
        model.CreatedBy = createdBy;
        model.BillingCreatedBy = createdBy;
        model.PageName = pageName;
        model.SupplierId = await manageRepo.CreateNewSuppiler_CopyAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<SupplierDto> Supplier_UpdateAsync(SupplierDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        await PrepareSupplierForUpsertAsync(model, cancellationToken);
        if (model.Provider_Status == 1) model.ProviderInactivated = DateTime.UtcNow;
        model.ModifiedBy = modifiedBy;
        await manageRepo.UpdateSupplierAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<SupplierDto> Supplier_Update_CopyAsync(SupplierDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        await PrepareSupplierForUpsertAsync(model, cancellationToken);
        if (model.Provider_Status == 1) model.ProviderInactivated = DateTime.UtcNow;
        model.ModifiedBy = modifiedBy;
        model.BillingModifiedBy = modifiedBy;
        await manageRepo.UpdateSupplier_CopyAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<SupplierDto> Supplier_DestroyAsync(SupplierDto model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(model.ParentProvider))
        {
            return model;
        }

        await manageRepo.DeleteSupplierAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return model;
    }

    public Task<IReadOnlyList<SupplierDto>> Supplier_GetAsync(CancellationToken cancellationToken = default)
        => manageRepo.GetSuppliersForSchedulingAsync(cancellationToken);

    public Task<IReadOnlyList<ManageListItemDto>> Vendor_ReadAsync(string? searchValue, string? contractingEntityId, int providerStatus, int departmentId, CancellationToken cancellationToken = default)
        => manageRepo.GetAllVendorAsync(searchValue, contractingEntityId, providerStatus, departmentId, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> AdjusterFirstName_ReadAsync(string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetAllAdjusterFirstNameAsync(searchValue, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> AdjusterLastName_ReadAsync(string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetAllAdjusterLastNameAsync(searchValue, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> County_ReadAsync(string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetAllCountyAsync(searchValue, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> VendorAddress_ReadAsync(string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetAllVendorAddressAsync(searchValue, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> BranchAddress_ReadAsync(string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetAllBranchAddressAsync(searchValue, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> VendorCity_ReadAsync(string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetAllVendorCityAsync(searchValue, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> NurseFirstName_ReadAsync(string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetAllNurseFirstNameAsync(searchValue, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> NurseLastName_ReadAsync(string? searchValue, CancellationToken cancellationToken = default)
        => manageRepo.GetAllNurseLastNameAsync(searchValue, cancellationToken);
    public Task<bool> VendorDocument_SaveAsync(DocumentDto model, CancellationToken cancellationToken = default)
        => manageRepo.SaveDocumentsAsync(model, cancellationToken);
    public Task<IReadOnlyList<DocumentDto>> VendorDocuments_ReadAsync(int? vendorId, CancellationToken cancellationToken = default)
        => (vendorId ?? 0) > 0 ? manageRepo.GetVendorDocumentsAsync(vendorId!.Value, cancellationToken) : Task.FromResult<IReadOnlyList<DocumentDto>>([]);
    public Task<bool> VendorDocument_DestroyAsync(DocumentDto model, CancellationToken cancellationToken = default)
        => manageRepo.DeleteDocumentAsync(model.DocumentId, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> VendorDocumentUploadTypes_ReadAsync(CancellationToken cancellationToken = default)
        => manageRepo.GetVendorDocumentUploadTypesAsync(cancellationToken);
    public Task<IReadOnlyList<ProviderContractedRatesDto>> ProviderRatesStatus_ReadAsync(int? supplierId, int? providerType, CancellationToken cancellationToken = default)
        => manageRepo.GetProviderRateaDetailAsync(supplierId, providerType, cancellationToken);
    public Task<int> CommonBillingEnableDisable_ReadAsync(int? supplierId, int? providerType, CancellationToken cancellationToken = default)
        => manageRepo.CommonBillingEnableDisableDetailsAsync(supplierId, providerType, cancellationToken);
    public Task<bool> UpdateParentCommonBillingAsync(int? supplierId, int? providerType, string modifiedBy, CancellationToken cancellationToken = default)
        => manageRepo.UpdateParentCommonBillingAsync(supplierId, providerType, modifiedBy, cancellationToken);
    #endregion

    #region....ProviderContractedRates......
    public Task<IReadOnlyList<ProviderContractedRatesDto>> ProviderContractedRates_ReadAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? InternalProid, bool VendorContractingEntityShow, bool VendorParentShow, string? txtProvider, int? ExternalId, string? TinId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
        => manageRepo.GetProviderContractedRatesAsync(startDate, endDate, departmentId, subspecialtyId, contractingEntityId, supplierId, jurisdiction, cptCode, InternalProid, VendorContractingEntityShow, VendorParentShow, txtProvider, ExternalId, TinId, fromDate, toDate, cancellationToken);
    public Task<int> ProviderContractedRates_CreateAsync(ProviderContractedRatesDto model, string createdBy, CancellationToken cancellationToken = default)
    {
        model.CreatedBy = createdBy;
        return manageRepo.SaveProviderContractedRatesAsync(model, cancellationToken);
    }
    public Task<bool> ProviderParent_ExistsAsync(ProviderContractedRatesDto model, CancellationToken cancellationToken = default)
        => manageRepo.IsParentRatesExistAsync(model, cancellationToken);
    public Task<int> ProviderContractedRates_updateAsync(ProviderContractedRatesDto model, string modifiedBy, CancellationToken cancellationToken = default)
    {
        model.ModifiedBy = modifiedBy;
        return manageRepo.SaveProviderContractedRatesAsync(model, cancellationToken);
    }
    public Task ProviderContractedRates_DestroyAsync(ProviderContractedRatesDto model, CancellationToken cancellationToken = default)
        => manageRepo.ProviderContractedRates_DestroyAsync(model, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> AllSupplier_ReadAsync(string? parentSearchValue, string? branchSearchValue, string? contractingEntityId, int parentid, CancellationToken cancellationToken = default)
        => manageRepo.GetAllSupplierIdAsync(parentSearchValue, branchSearchValue, contractingEntityId, parentid, cancellationToken);
    public Task<IReadOnlyList<SupplierDto>> AllParenBranch_ReadAsync(bool? IsParent, bool? IsBranch, CancellationToken cancellationToken = default)
        => manageRepo.GetAllParenBranch_ReadAsync(IsParent, IsBranch, cancellationToken);
    public Task<IReadOnlyList<ProviderContractedRatesExportDto>> ProviderRatesExportExcelAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? InternalProid, bool VendorContractingEntityShow, bool VendorParentShow, string? txtProvider, int? ExternalId, string? TinId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
        => manageRepo.GetProviderContractedRatesExportAsync(startDate, endDate, departmentId, subspecialtyId, contractingEntityId, supplierId, jurisdiction, cptCode, InternalProid, VendorContractingEntityShow, VendorParentShow, txtProvider, ExternalId, TinId, fromDate, toDate, cancellationToken);
    public Task<IReadOnlyList<ProviderContractedRatesCompIQExportDto>> ProviderRatesCompIQExportAsync(DateTime? startDate, DateTime? endDate, string? departmentId, int? subspecialtyId, int? contractingEntityId, int? supplierId, string? jurisdiction, string? cptCode, string? InternalProid, bool VendorContractingEntityShow, bool VendorParentShow, string? txtProvider, int? ExternalId, string? TinId, DateTime? fromDate, DateTime? toDate, IReadOnlyList<ProviderContractedRatesDto>? selectedData, CancellationToken cancellationToken = default)
        => manageRepo.GetProviderContractedRatesCompiqExportAsync(startDate, endDate, departmentId, subspecialtyId, contractingEntityId, supplierId, jurisdiction, cptCode, InternalProid, VendorContractingEntityShow, VendorParentShow, txtProvider, ExternalId, TinId, fromDate, toDate, selectedData, cancellationToken);
    public Task<IReadOnlyList<ManageListItemDto>> DiscountType_ReadAsync(CancellationToken cancellationToken = default)
        => manageRepo.GetAllDiscountTypeAsync(cancellationToken);

    public async Task<IReadOnlyList<ProviderContractedRatesCompIQExportDto>> GenerateCIQUploadExcelAsync(IReadOnlyList<ProviderContractedRatesDto> selectedData, CancellationToken cancellationToken = default)
    {
        var data = await manageRepo.GetProviderContractedRatesCompiqExportAsync(null, null, "0", null, null, null, null, null, null, false, false, string.Empty, null, string.Empty, null, null, selectedData, cancellationToken);
        lock (compIqUploadData)
        {
            compIqUploadData.Clear();
            compIqUploadData.AddRange(data);
        }
        return data;
    }

    public Task<bool> ExportProviderRatesCompiqExcelAndUploadAsync(IReadOnlyList<ProviderContractedRatesCompIQExportDto> data, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        lock (compIqUploadData)
        {
            compIqUploadData.Clear();
            compIqUploadData.AddRange(data);
        }
        return Task.FromResult(true);
    }

    public Task<bool> PhysicianAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<IReadOnlyList<PhysicianDto>> Physician_ReadAsync(CancellationToken cancellationToken = default)
        => manageRepo.GetAllPhysicianAsync(cancellationToken);

    public Task<IReadOnlyList<ManageStatusLookupDto>> VendorStatus_ReadAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ManageStatusLookupDto>>(
        [
            new(1, "Contracted"),
            new(5, "Credit Card"),
            new(3, "Invoice"),
            new(2, "LOA"),
            new(4, "Other")
        ]);

    public async Task<IReadOnlyList<ManageStatusLookupDto>> ProviderStatusReadAsync(CancellationToken cancellationToken = default)
    {
        var all = await GetProviderStatusListAsync(cancellationToken);
        return all.Where(x => x.Value != 2).OrderBy(x => x.Value).ToList();
    }

    public async Task<IReadOnlyList<ManageStatusLookupDto>> ProviderStatusReaderAsync(CancellationToken cancellationToken = default)
    {
        var all = await GetProviderStatusListAsync(cancellationToken);
        return all.Where(x => x.Value != 2 && x.Value != 3).OrderBy(x => x.Value).ToList();
    }

    #endregion

    public Task<IReadOnlyList<BillingRegionsDto>> BillingRegions_ReadAsync(CancellationToken cancellationToken = default) => manageRepo.GetAllBillingRegionsAsync(cancellationToken);

    public async Task<bool> IsBillingRegionExistsAsync(string regionName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(regionName)) throw new ArgumentException("Region name is required.", nameof(regionName));
        return !await manageRepo.IsBillingRegionExistsAsync(regionName, excludingBillingRegionId: null, cancellationToken);
    }

    public async Task BillingRegions_CreateAsync(CreateBillingRegionRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.CreatedBy)) throw new ArgumentException("CreatedBy is required.", nameof(request.CreatedBy));
        if (string.IsNullOrWhiteSpace(request.BillingRegionName)) throw new ArgumentException("BillingRegionName is required.", nameof(request.BillingRegionName));
        if (await manageRepo.IsBillingRegionExistsAsync(request.BillingRegionName, excludingBillingRegionId: null, cancellationToken))
            throw new InvalidOperationException("Billing region already exists.");

        var model = new BillingRegionsDto(
            BillingRegionId: 0,
            BillingRegionName: request.BillingRegionName.Trim(),
            Jurisdiction: request.Jurisdiction?.Trim() ?? string.Empty,
            IsActive: request.IsActive,
            CreatedDate: DateTime.UtcNow,
            CreatedBy: request.CreatedBy.Trim(),
            ModifiedDate: null,
            ModifiedBy: null);

        await manageRepo.CreateBillingRegionAsync(model, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ManagePermissionDto>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var raw = await manageRepo.GetRolePermissionsAsync(roleId, cancellationToken);
        var names = ManagePermissionSet.Normalize(raw.Select(x => x.PermissionName));
        return names.Select(n => new ManagePermissionDto(roleId, n)).ToList();
    }

    public async Task<bool> UpsertRoleAsync(int roleId, UpsertRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy)) throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        var role = await manageRepo.GetRoleAsync(roleId, cancellationToken);
        if (role is null) return false;
        var changed = request.Destroy ? role.TryDestroy() : role.TryRename(request.RoleName);
        if (!changed) return true;
        await manageRepo.UpdateRoleAsync(role.RoleId, role.RoleName, role.IsDeleted, request.ModifiedBy, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpsertCompanyAsync(int companyId, UpsertCompanyRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy)) throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        var company = await manageRepo.GetCompanyAsync(companyId, cancellationToken);
        if (company is null) return false;
        var changed = request.Destroy ? company.TryDestroy() : company.TryUpdateCode(request.CompanyCode);
        if (!changed) return true;
        if (!request.Destroy && await manageRepo.CompanyCodeExistsAsync(company.CompanyCode, company.CompanyId, cancellationToken)) throw new InvalidOperationException("Company code already exists.");
        await manageRepo.UpdateCompanyAsync(company.CompanyId, company.CompanyCode, company.IsDeleted, request.ModifiedBy, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpsertCptCodeAsync(long cptCodeId, UpsertCptCodeRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy)) throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        var cpt = await manageRepo.GetCptCodeAsync(cptCodeId, cancellationToken);
        if (cpt is null) return false;
        var changed = request.Destroy ? cpt.TryDestroy() : cpt.TryUpdate(request.CptCode, request.SubSpecialityId, request.Modifier);
        if (!changed) return true;
        if (!request.Destroy && await manageRepo.CptCodeExistsAsync(cpt.CptCode, cpt.SubSpecialityId, cpt.Modifier, cpt.CptCodeId, cancellationToken)) throw new InvalidOperationException("CPT code already exists for this modifier/subspeciality.");
        await manageRepo.UpdateCptCodeAsync(cpt.CptCodeId, cpt.CptCode, cpt.SubSpecialityId, cpt.Modifier, cpt.IsDeleted, request.ModifiedBy, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<ManageParityPreviewResponse> PreviewAsync(ManageParityPreviewRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new ManageParityPreviewResponse(ProviderRatePolicy.IsValid(request.CompXRate, request.ProviderRate)));

    private static string SanitizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return string.Empty;
        var chars = phone.Where(char.IsDigit).ToArray();
        return new string(chars);
    }

    private static string GetDepartmentType(int department)
        => department switch
        {
            1 => "TRA",
            3 => "PM",
            2 => "LAN",
            4 => "DS",
            6 => "HHS",
            5 => "DME",
            7 => "XS",
            _ => string.Empty
        };

    private async Task PrepareSupplierForUpsertAsync(SupplierDto model, CancellationToken cancellationToken)
    {
        model.SubSpecialtyIds = ConvertStringArrayToString(model.SubSpecialty);
        if (!string.IsNullOrWhiteSpace(model.SubSpecialtyIds))
        {
            var ids = model.SubSpecialtyIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse)
                .ToList();
            model.SubSpecialty = (await manageRepo.GetSubSpecialityNamesByIdsAsync(ids, cancellationToken)).ToList();
        }

        if (string.IsNullOrWhiteSpace(model.ContactPhone)) model.ContactPhone = model.HdnPhone;
        if (string.IsNullOrWhiteSpace(model.BillingFax)) model.BillingFax = model.HdnBgFax;
        if (string.IsNullOrWhiteSpace(model.BillingPhone)) model.BillingPhone = model.HdnBgPhone;

        if (model.ProviderAndBranchRadioButton == 2)
        {
            model.BranchId = model.ParentProvider;
            model.ParentId = string.IsNullOrWhiteSpace(model.BranchId) ? null : Convert.ToInt32(model.BranchId);
        }
    }

    private static string? ConvertStringArrayToString(ICollection<string>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(",", values);
    }

    private static Task<IReadOnlyList<ManageStatusLookupDto>> GetProviderStatusListAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return Task.FromResult<IReadOnlyList<ManageStatusLookupDto>>(
        [
            new(1, "Inactive"),
            new(0, "Active"),
            new(3, "All"),
            new(2, "Pending(Old)")
        ]);
    }
}
