using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Lifetrons.Erp.Activity.Controllers;
using Lifetrons.Erp.Admin.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.People.Controllers;
using Lifetrons.Erp.Product.Controllers;
using Lifetrons.Erp.Sales.Controllers;
using Lifetrons.Erp.SysAdmin.Controllers;
using Lifetrons.Erp.Works.Controllers;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Ef6.Factories;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Service;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.File.Controllers;
using Lifetrons.Erp.Web.Models.Repository;
using Lifetrons.Erp.Web.Domain.Twilio.MarketingNotifications;
using Lifetrons.Erp.Web.Domain.Twilio.Notifications;

namespace Lifetrons.Erp.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IDataContextAsync, Lifetrons.Erp.Data.Entities>(new PerRequestLifetimeManager());
            container.RegisterType<IDataContext, Lifetrons.Erp.Data.Entities>(new PerRequestLifetimeManager());

            container.RegisterType<IRepositoryProvider, RepositoryProvider>(new PerRequestLifetimeManager(),
                new InjectionConstructor(new object[] { new RepositoryFactories() }));

            container.RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerRequestLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new PerRequestLifetimeManager());

            container.RegisterType<IRepositoryAsync<AspNetUser>, Repository<AspNetUser>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Organization>, Repository<Organization>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<EmailConfig>, Repository<EmailConfig>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<JoiningRequest>, Repository<JoiningRequest>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Department>, Repository<Department>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Team>, Repository<Team>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Hierarchy>, Repository<Hierarchy>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProductFamily>, Repository<ProductFamily>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProductType>, Repository<ProductType>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Lifetrons.Erp.Data.Product>, Repository<Lifetrons.Erp.Data.Product>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<PriceBook>, Repository<PriceBook>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<PriceBookLineItem>, Repository<PriceBookLineItem>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Address>, Repository<Address>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Account>, Repository<Account>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<AccountType>, Repository<AccountType>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Rating>, Repository<Rating>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Industry>, Repository<Industry>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Ownership>, Repository<Ownership>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Contact>, Repository<Contact>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<LeadSource>, Repository<LeadSource>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<LeadStatu>, Repository<LeadStatu>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Level>, Repository<Level>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<CampaignType>, Repository<CampaignType>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<CampaignStatu>, Repository<CampaignStatu>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Campaign>, Repository<Campaign>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<CampaignMember>, Repository<CampaignMember>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<CampaignMemberStatu>, Repository<CampaignMemberStatu>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Lead>, Repository<Lead>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<TaskStatu>, Repository<TaskStatu>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Priority>, Repository<Priority>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Task>, Repository<Task>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<WeightUnit>, Repository<WeightUnit>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<OpportunityType>, Repository<OpportunityType>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Opportunity>, Repository<Opportunity>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<OpportunityLineItem>, Repository<OpportunityLineItem>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Stage>, Repository<Stage>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<DeliveryStatu>, Repository<DeliveryStatu>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Quote>, Repository<Quote>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<QuoteLineItem>, Repository<QuoteLineItem>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<QuoteStatu>, Repository<QuoteStatu>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Contract>, Repository<Contract>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Order>, Repository<Order>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<OrderLineItem>, Repository<OrderLineItem>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<InvoiceStatu>, Repository<InvoiceStatu>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Invoice>, Repository<Invoice>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<InvoiceLineItem>, Repository<InvoiceLineItem>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<CaseStatu>, Repository<CaseStatu>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<CaseReason>, Repository<CaseReason>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Case>, Repository<Case>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Target>, Repository<Target>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<spUserOpenWork_Result>, Repository<spUserOpenWork_Result>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<spUserPerformanceComaprisonMonthly_Result>, Repository<spUserPerformanceComaprisonMonthly_Result>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<spTeamOpenWork_Result>, Repository<spTeamOpenWork_Result>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<spTeamPerformanceComaprisonMonthly_Result>, Repository<spTeamPerformanceComaprisonMonthly_Result>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<spDepartmentOpenWork_Result>, Repository<spDepartmentOpenWork_Result>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<spDepartmentPerformanceComaprisonMonthly_Result>, Repository<spDepartmentPerformanceComaprisonMonthly_Result>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<spOrgOpenWork_Result>, Repository<spOrgOpenWork_Result>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<spOrgPerformanceComaprisonMonthly_Result>, Repository<spOrgPerformanceComaprisonMonthly_Result>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<NoticeBoard>, Repository<NoticeBoard>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ItemClassification>, Repository<ItemClassification>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ItemCategory>, Repository<ItemCategory>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ItemType>, Repository<ItemType>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ItemGroup>, Repository<ItemGroup>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ItemSubGroup>, Repository<ItemSubGroup>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<CostingGroup>, Repository<CostingGroup>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<CostingSubGroup>, Repository<CostingSubGroup>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Nature>, Repository<Nature>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Shape>, Repository<Shape>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Colour>, Repository<Colour>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Style>, Repository<Style>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Item>, Repository<Item>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<EnterpriseStage>, Repository<EnterpriseStage>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Process>, Repository<Process>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProcessTimeConfig>, Repository<ProcessTimeConfig>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<BOM>, Repository<BOM>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<BOMLineItem>, Repository<BOMLineItem>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Operation>, Repository<Operation>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<OperationBOMLineItem>, Repository<OperationBOMLineItem>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Employee>, Repository<Employee>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<StockReceiptHead>, Repository<StockReceiptHead>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<StockItemReceipt>, Repository<StockItemReceipt>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<StockProductReceipt>, Repository<StockProductReceipt>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<StockIssueHead>, Repository<StockIssueHead>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<StockItemIssue>, Repository<StockItemIssue>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<StockProductIssue>, Repository<StockProductIssue>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<JobIssueHead>, Repository<JobIssueHead>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<JobProductIssue>, Repository<JobProductIssue>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<JobItemIssue>, Repository<JobItemIssue>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<JobReceiptHead>, Repository<JobReceiptHead>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<JobProductReceipt>, Repository<JobProductReceipt>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<JobItemReceipt>, Repository<JobItemReceipt>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Dispatch>, Repository<Dispatch>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<DispatchLineItem>, Repository<DispatchLineItem>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProdPlanDetail>, Repository<ProdPlanDetail>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProdPlanRawBooking>, Repository<ProdPlanRawBooking>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProcurementRequest>, Repository<ProcurementRequest>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProcurementRequestDetail>, Repository<ProcurementRequestDetail>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProcurementOrder>, Repository<ProcurementOrder>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<ProcurementOrderDetail>, Repository<ProcurementOrderDetail>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Attendance>, Repository<Attendance>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Media>, Repository<Media>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<Lifetrons.Erp.Data.File>, Repository<Lifetrons.Erp.Data.File>>(new PerRequestLifetimeManager());
            container.RegisterType<IRepositoryAsync<FileRateTable>, Repository<FileRateTable>>(new PerRequestLifetimeManager());

            container.RegisterType<IAspNetUserService, AspNetUserService>(new PerRequestLifetimeManager());
            container.RegisterType<IOrganizationService, OrganizationService>(new PerRequestLifetimeManager());
            container.RegisterType<IEmailConfigService, EmailConfigService>(new PerRequestLifetimeManager());
            container.RegisterType<IJoiningRequestService, JoiningRequestService>(new PerRequestLifetimeManager());
            container.RegisterType<IDepartmentService, DepartmentService>(new PerRequestLifetimeManager());
            container.RegisterType<ITeamService, TeamService>(new PerRequestLifetimeManager());
            container.RegisterType<IHierarchyService, HierarchyService>(new PerRequestLifetimeManager());
            container.RegisterType<IProductFamilyService, ProductFamilyService>(new PerRequestLifetimeManager());
            container.RegisterType<IProductTypeService, ProductTypeService>(new PerRequestLifetimeManager());
            container.RegisterType<IProductService, ProductService>(new PerRequestLifetimeManager());
            container.RegisterType<IPriceBookService, PriceBookService>(new PerRequestLifetimeManager());
            container.RegisterType<IPriceBookLineItemService, PriceBookLineItemService>(new PerRequestLifetimeManager());
            container.RegisterType<IAddressService, AddressService>(new PerRequestLifetimeManager());
            container.RegisterType<IAccountService, AccountService>(new PerRequestLifetimeManager());
            container.RegisterType<IAccountTypeService, AccountTypeService>(new PerRequestLifetimeManager());
            container.RegisterType<IRatingService, RatingService>(new PerRequestLifetimeManager());
            container.RegisterType<IIndustryService, IndustryService>(new PerRequestLifetimeManager());
            container.RegisterType<IOwnershipService, OwnershipService>(new PerRequestLifetimeManager());
            container.RegisterType<IContactService, ContactService>(new PerRequestLifetimeManager());
            container.RegisterType<ILeadSourceService, LeadSourceService>(new PerRequestLifetimeManager());
            container.RegisterType<ILeadStatuService, LeadStatuService>(new PerRequestLifetimeManager());
            container.RegisterType<ILevelService, LevelService>(new PerRequestLifetimeManager());
            container.RegisterType<ICampaignTypeService, CampaignTypeService>(new PerRequestLifetimeManager());
            container.RegisterType<ICampaignStatuService, CampaignStatuService>(new PerRequestLifetimeManager());
            container.RegisterType<ICampaignService, CampaignService>(new PerRequestLifetimeManager());
            container.RegisterType<ICampaignMemberService, CampaignMemberService>(new PerRequestLifetimeManager());
            container.RegisterType<ICampaignMemberStatusService, CampaignMemberStatusService>(new PerRequestLifetimeManager());
            container.RegisterType<ILeadService, LeadService>(new PerRequestLifetimeManager());
            container.RegisterType<ITaskStatuService, TaskStatuService>(new PerRequestLifetimeManager());
            container.RegisterType<IPriorityService, PriorityService>(new PerRequestLifetimeManager());
            container.RegisterType<ITaskService, TaskService>(new PerRequestLifetimeManager());
            container.RegisterType<IWeightUnitService, WeightUnitService>(new PerRequestLifetimeManager());
            container.RegisterType<IOpportunityTypeService, OpportunityTypeService>(new PerRequestLifetimeManager());
            container.RegisterType<IOpportunityService, OpportunityService>(new PerRequestLifetimeManager());
            container.RegisterType<IOpportunityLineItemService, OpportunityLineItemService>(new PerRequestLifetimeManager());
            container.RegisterType<IStageService, StageService>(new PerRequestLifetimeManager());
            container.RegisterType<IDeliveryStatuService, DeliveryStatuService>(new PerRequestLifetimeManager());
            container.RegisterType<IQuoteService, QuoteService>(new PerRequestLifetimeManager());
            container.RegisterType<IQuoteLineItemService, QuoteLineItemService>(new PerRequestLifetimeManager());
            container.RegisterType<IQuoteStatusService, QuoteStatusService>(new PerRequestLifetimeManager());
            container.RegisterType<IContractService, ContractService>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderService, OrderService>(new PerRequestLifetimeManager());
            container.RegisterType<IOrderLineItemService, OrderLineItemService>(new PerRequestLifetimeManager());
            container.RegisterType<IInvoiceStatusService, InvoiceStatusService>(new PerRequestLifetimeManager());
            container.RegisterType<IInvoiceService, InvoiceService>(new PerRequestLifetimeManager());
            container.RegisterType<IInvoiceLineItemService, InvoiceLineItemService>(new PerRequestLifetimeManager());
            container.RegisterType<ICaseStatusService, CaseStatusService>(new PerRequestLifetimeManager());
            container.RegisterType<ICaseReasonService, CaseReasonService>(new PerRequestLifetimeManager());
            container.RegisterType<ICaseService, CaseService>(new PerRequestLifetimeManager());
            container.RegisterType<ITargetService, TargetService>(new PerRequestLifetimeManager());
            container.RegisterType<IStoredProcedureService, StoredProcedureService>(new PerRequestLifetimeManager());
            container.RegisterType<INoticeBoardService, NoticeBoardService>(new PerRequestLifetimeManager());
            container.RegisterType<IItemClassificationService, ItemClassificationService>(new PerRequestLifetimeManager());
            container.RegisterType<IItemCategoryService, ItemCategoryService>(new PerRequestLifetimeManager());
            container.RegisterType<IItemTypeService, ItemTypeService>(new PerRequestLifetimeManager());
            container.RegisterType<IItemGroupService, ItemGroupService>(new PerRequestLifetimeManager());
            container.RegisterType<IItemSubGroupService, ItemSubGroupService>(new PerRequestLifetimeManager());
            container.RegisterType<ICostingGroupService, CostingGroupService>(new PerRequestLifetimeManager());
            container.RegisterType<ICostingSubGroupService, CostingSubGroupService>(new PerRequestLifetimeManager());
            container.RegisterType<INatureService, NatureService>(new PerRequestLifetimeManager());
            container.RegisterType<IShapeService, ShapeService>(new PerRequestLifetimeManager());
            container.RegisterType<IColourService, ColourService>(new PerRequestLifetimeManager());
            container.RegisterType<IStyleService, StyleService>(new PerRequestLifetimeManager());
            container.RegisterType<IItemService, ItemService>(new PerRequestLifetimeManager());
            container.RegisterType<IProcessService, ProcessService>(new PerRequestLifetimeManager());
            container.RegisterType<IProcessTimeConfigService, ProcessTimeConfigService>(new PerRequestLifetimeManager());
            container.RegisterType<IEnterpriseStageService, EnterpriseStageService>(new PerRequestLifetimeManager());
            container.RegisterType<IBOMService, BOMService>(new PerRequestLifetimeManager());
            container.RegisterType<IBOMLineItemService, BOMLineItemService>(new PerRequestLifetimeManager());
            container.RegisterType<IOperationService, OperationService>(new PerRequestLifetimeManager());
            container.RegisterType<IOperationBOMLineItemService, OperationBOMLineItemService>(new PerRequestLifetimeManager());
            container.RegisterType<IEmployeeService, EmployeeService>(new PerRequestLifetimeManager());
            container.RegisterType<IStockReceiptHeadService, StockReceiptHeadService>(new PerRequestLifetimeManager());
            container.RegisterType<IStockItemReceiptService, StockItemReceiptService>(new PerRequestLifetimeManager());
            container.RegisterType<IStockProductReceiptService, StockProductReceiptService>(new PerRequestLifetimeManager());
            container.RegisterType<IStockIssueHeadService, StockIssueHeadService>(new PerRequestLifetimeManager());
            container.RegisterType<IStockItemIssueService, StockItemIssueService>(new PerRequestLifetimeManager());
            container.RegisterType<IStockProductIssueService, StockProductIssueService>(new PerRequestLifetimeManager());
            container.RegisterType<IJobIssueHeadService, JobIssueHeadService>(new PerRequestLifetimeManager());
            container.RegisterType<IJobProductIssueService, JobProductIssueService>(new PerRequestLifetimeManager());
            container.RegisterType<IJobItemIssueService, JobItemIssueService>(new PerRequestLifetimeManager());
            container.RegisterType<IJobReceiptHeadService, JobReceiptHeadService>(new PerRequestLifetimeManager());
            container.RegisterType<IJobProductReceiptService, JobProductReceiptService>(new PerRequestLifetimeManager());
            container.RegisterType<IJobItemReceiptService, JobItemReceiptService>(new PerRequestLifetimeManager());
            container.RegisterType<IDispatchService, DispatchService>(new PerRequestLifetimeManager());
            container.RegisterType<IDispatchLineItemService, DispatchLineItemService>(new PerRequestLifetimeManager());
            container.RegisterType<IProdPlanDetailService, ProdPlanDetailService>(new PerRequestLifetimeManager());
            container.RegisterType<IProdPlanRawBookingService, ProdPlanRawBookingService>(new PerRequestLifetimeManager());
            container.RegisterType<IProcurementRequestService, ProcurementRequestService>(new PerRequestLifetimeManager());
            container.RegisterType<IProcurementRequestDetailService, ProcurementRequestDetailService>(new PerRequestLifetimeManager());
            container.RegisterType<IProcurementOrderService, ProcurementOrderService>(new PerRequestLifetimeManager());
            container.RegisterType<IProcurementOrderDetailService, ProcurementOrderDetailService>(new PerRequestLifetimeManager());
            container.RegisterType<IAttendanceService, AttendanceService>(new PerRequestLifetimeManager());
            container.RegisterType<IMediaService, MediaService>(new PerRequestLifetimeManager());
            container.RegisterType<IFileService, FileService>(new PerRequestLifetimeManager());
            container.RegisterType<IFileRateTableService, FileRateTableService>(new PerRequestLifetimeManager());
            container.RegisterType<IFileRateTableService, FileRateTableService>(new PerRequestLifetimeManager());
            container.RegisterType<ISubscribersRepository, SubscribersRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IAdministratorsRepository, AdministratorsRepository>(new PerRequestLifetimeManager());
            container.RegisterType<ISubscribersNotificationService, SubscribersNotificationService>(new PerRequestLifetimeManager());
            container.RegisterType<IRestClient, RestClient>(new PerRequestLifetimeManager());

            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<OrganizationController>(new PerRequestLifetimeManager());
            container.RegisterType<JoiningRequestController>(new PerRequestLifetimeManager());
            container.RegisterType<DepartmentController>(new PerRequestLifetimeManager());
            container.RegisterType<TeamController>(new PerRequestLifetimeManager());
            container.RegisterType<HierarchyController>(new PerRequestLifetimeManager());
            container.RegisterType<RolesController>(new PerRequestLifetimeManager());
            container.RegisterType<ProductFamilyController>(new PerRequestLifetimeManager());
            container.RegisterType<ProductTypeController>(new PerRequestLifetimeManager());
            container.RegisterType<ProductController>(new PerRequestLifetimeManager());
            container.RegisterType<PriceBookController>(new PerRequestLifetimeManager());
            container.RegisterType<PriceBookLineItemController>(new PerRequestLifetimeManager());
            container.RegisterType<AddressController>(new PerRequestLifetimeManager());
            container.RegisterType<SAccountController>(new PerRequestLifetimeManager());
            container.RegisterType<ContactController>(new PerRequestLifetimeManager());
            container.RegisterType<CampaignController>(new PerRequestLifetimeManager());
            container.RegisterType<CampaignMemberController>(new PerRequestLifetimeManager());
            container.RegisterType<LeadController>(new PerRequestLifetimeManager());
            container.RegisterType<QuoteController>(new PerRequestLifetimeManager());
            container.RegisterType<QuoteLineItemController>(new PerRequestLifetimeManager());
            container.RegisterType<TaskController>(new PerRequestLifetimeManager());
            container.RegisterType<OpportunityController>(new PerRequestLifetimeManager());
            container.RegisterType<OpportunityLineItemController>(new PerRequestLifetimeManager());
            container.RegisterType<ContractController>(new PerRequestLifetimeManager());
            container.RegisterType<OrderController>(new PerRequestLifetimeManager());
            container.RegisterType<OrderLineItemController>(new PerRequestLifetimeManager());
            container.RegisterType<InvoiceController>(new PerRequestLifetimeManager());
            container.RegisterType<InvoiceLineItemController>(new PerRequestLifetimeManager());
            container.RegisterType<CaseController>(new PerRequestLifetimeManager());
            container.RegisterType<AdminController>(new PerRequestLifetimeManager());
            container.RegisterType<TargetController>(new PerRequestLifetimeManager());
            container.RegisterType<NoticeBoardController>(new PerRequestLifetimeManager());
            container.RegisterType<ItemController>(new PerRequestLifetimeManager());
            container.RegisterType<ProcessController>(new PerRequestLifetimeManager());
            container.RegisterType<BOMController>(new PerRequestLifetimeManager());
            container.RegisterType<BOMLineItemController>(new PerRequestLifetimeManager());
            container.RegisterType<MediaController>(new PerRequestLifetimeManager());
            container.RegisterType<FileController>(new PerRequestLifetimeManager());
            container.RegisterType<FileController>();
            container.RegisterType<FileRateTableController>();
            container.RegisterType<NotificationsController>();
            container.RegisterType<SubscribersController>();

            container.RegisterType<BarCodeSampleController>(new PerRequestLifetimeManager());
            //container.RegisterType<EmailController>(new PerRequestLifetimeManager());
            //container.RegisterType<DashboardController>(new PerRequestLifetimeManager());
        }
    }
}
