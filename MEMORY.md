# ApnaPashu - Single Source of Truth & Development Memory (MEMORY.md)

This document is the **Single Source of Truth** for the entire **ApnaPashu** codebase across both Backend (`APNAPASHU_BackEnd`) and Frontend (`APNAPASHU_FrontEnd`). All future development MUST read, follow, and uphold the architecture, patterns, reusable inventory, design guidelines, and rules defined in this document.

---

## 1. Project Architecture

### Overall Project Overview
**ApnaPashu** is a digital livestock/cattle trading marketplace and administration ecosystem connecting **Buyers**, **Sellers**, and **Platform Administrators**.

* **Backend Repository:** [`APNAPASHU_BackEnd`](file:///e:/Projects/ApnaPashu/APNAPASHU_BackEnd) (ASP.NET Core .NET 8.0 Web API)
* **Frontend Repository:** [`APNAPASHU_FrontEnd`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd) (Angular 21 Standalone Components + Tailwind CSS v4)

---

### Backend Architecture & Layering Pattern
The backend adheres strictly to a **5-Layer Clean Architecture**:

```
[APNAPASHU.API] (Web/Mobile Controllers & Middleware)
     │
     ▼
[APNAPASHU.ServiceContract] / [APNAPASHU.Service] (Business Logic & Validation)
     │
     ▼
[APNAPASHU.RepositoryContract] / [APNAPASHU.Repository] (Dapper & EF Core Data Access)
     │
     ▼
[APNAPASHU.DataContract] (Entities, Request/Response DTOs, Enums)
     │
     ▼
[APNAPASHU.Common] (Cross-cutting Utilities, Constants, Helpers)
```

#### Layer Responsibilities:
1. **`APNAPASHU` (API Layer):** Controller routing, request model validation, JWT HTTP-only cookie authentication, OpenAPI/Swagger docs, SignalR Hubs (`/hubs/chat`), exception filters.
2. **`APNAPASHU.ServiceContract` & `APNAPASHU.Service`:** Implements `BaseService`. Handles business logic, file upload to Cloudflare R2 (`R2Uploader`), SMTP emails, and DTO transformation.
3. **`APNAPASHU.RepositoryContract` & `APNAPASHU.Repository`:** Implements `BaseRepository`. Handles data access via **Dapper** (for high-performance stored procedures and raw SQL queries) and EF Core `AppDbContext`.
4. **`APNAPASHU.DataContract`:** Contains domain entities (`Entity/`), request models (`UpsertModel`), response models (`ResponseModel`), shared DTOs (`FilterDto`, `CommonAuditDto`, `JsonModel<T>`).
5. **`APNAPASHU.Common`:** Shared constants, string helpers, and common utilities.

---

### Frontend Architecture & Module Structure
The frontend is built with **Angular 21** using **Standalone Components** and **Angular Signals** for state management:

```
src/app/
├── core/                   # Guards (authGuard), Core Services (AuthService)
├── shared/                 # Reusable UI Components, Base Services, Interceptors, Pipes, Models
│   ├── components/         # data-table, modal, toaster, cards, input, label, form
│   ├── interceptors/       # authInterceptor (JWT cookie & 401 handling)
│   ├── services/           # base-crud.service.ts, toaster.service.ts, chat.service.ts
│   └── models/             # Shared TypeScript interfaces & DTOs
├── admin/                  # Admin Portal (Role-based menu, category, role, permission management)
│   ├── category-management/
│   ├── role-management/
│   ├── permission-management/
│   └── services/           # Admin-specific services
└── users/                  # Buyer & Seller Portal
    ├── buyer/              # Browse animals, favorites, buyer dashboard, inquiries
    ├── seller/             # Seller dashboard, posted animals, inquiries
    ├── public/             # Public landing pages, pet index
    └── shared/             # Chat component, profile settings
```

---

### Authentication & Authorization Architecture
* **Token Storage:** Authenticated via **JWT Tokens** stored in HTTP-Only cookies (`AuthToken`).
* **Backend Authentication Flow:**
  * Configured in `Program.cs` with `JwtBearerDefaults`.
  * Cookie extraction is handled dynamically via `OnMessageReceived` event (`context.Request.Cookies["AuthToken"]`).
  * `GetAuthenticatedUserId()` in `BaseController` extracts user identity from JWT claims (`ClaimTypes.NameIdentifier` or `"UserId"`).
* **Frontend Authentication & Viewport Isolation:**
  * [`AuthService`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/core/services/auth.service.ts) manages state via Angular Signals (`currentUser`, `isLoggedIn`, `actualRole`, `userRole`).
  * [`authGuard`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/core/guards/auth.guard.ts) enforces **Strict URL Viewport Isolation**:
    * Buyers attempting to access `/user/seller/*` are redirected to `/user/buyer/dashboard`.
    * Sellers attempting to access `/user/buyer/*` are redirected to `/user/seller/dashboard`.
    * Non-staff users attempting to access `/admin/*` are redirected to their respective user dashboard.
  * Dual-Role Users (`sellerbuyerboth`): Allows seamless role toggling between buyer and seller viewports via `AuthService.toggleRole()`.
  * [`authInterceptor`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/shared/interceptors/auth.interceptor.ts) automatically attaches `withCredentials: true` and catches `401 Unauthorized` responses.

---

## 2. Backend Technical Standards & Conventions

### Framework & Target Runtime
* **Target Framework:** `.NET 8.0` (`<TargetFramework>net8.0</TargetFramework>`)
* **Nullable Reference Types:** Enabled (`<Nullable>enable</Nullable>`)
* **Implicit Usings:** Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)

### Key NuGet Packages
* `Dapper` (v2.1.15) - High performance ORM for data querying.
* `Microsoft.EntityFrameworkCore.SqlServer` (v8.0.0) - EF Core SQL Server context.
* `Microsoft.AspNetCore.Authentication.JwtBearer` (v8.0.0) - JWT Token authentication.
* `FluentValidation` (v11.8.1) - Request validation.
* `Swashbuckle.AspNetCore` (v6.8.0) - Swagger API documentation.

---

### Standard Response Envelope (`JsonModel<T>`)
Every controller action MUST return responses formatted within `JsonModel<T>` ([`CommonModels.cs`](file:///e:/Projects/ApnaPashu/APNAPASHU_BackEnd/APNAPASHU.DataContract/Models/CommonModels.cs)):

```csharp
public class JsonModel<T>
{
    public string? AppError { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int? StatusCode { get; set; }
    public string? AccessToken { get; set; }
}
```

### Response Messages Standard (`ResponseMessages`)
Do NOT hardcode raw string messages in Services or Repositories. Consume constants from [`ResponseMessages`](file:///e:/Projects/ApnaPashu/APNAPASHU_BackEnd/APNAPASHU.Common/Messages/ResponseMessages.cs):
* `ResponseMessages.fetchedSuccessfully` ("Data fetched sccessfully")
* `ResponseMessages.insertedSuccessfully` ("Data inserted sccessfully")
* `ResponseMessages.updatedSuccessfully` ("Data updated sccessfully")
* `ResponseMessages.statusUpdated` ("Status updated successfully")
* `ResponseMessages.deletedSuccessfully` ("Data deleted successfully")
* `ResponseMessages.NotFound` ("Resource not found")
* `ResponseMessages.Error` / `ResponseMessages.Success`

---

### Pagination & Filter DTO (`FilterDto`)
All listing and paginated requests MUST consume [`FilterDto`](file:///e:/Projects/ApnaPashu/APNAPASHU_BackEnd/APNAPASHU.DataContract/Models/FilterDto.cs):

```csharp
public class FilterDto
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortCulumn { get; set; }
    public string? SortDirection { get; set; } = "DESC";
    public string? ModuleKey { get; set; }
    public int? RoleId { get; set; }
}
```

---

### Common Audit Fields (`CommonAuditDto`)
Entities and models with database auditing metadata inherit from or contain:
* `Id` (`int`)
* `IsActive` (`bool?`)
* `IsDeleted` (`bool?`)
* `CreatedDate` / `CreatedBy`
* `UpdatedDate` / `UpdatedBy`
* `DeletedDate` / `DeletedBy`
* `TotalRecords` (`int?`) - Used for SQL pagination total count returned from stored procedures.

---

### Controller Conventions (`BaseController`)
* Inherit from [`BaseController`](file:///e:/Projects/ApnaPashu/APNAPASHU_BackEnd/APNAPASHU/Controllers/BaseController.cs).
* Decorate with `[ApiController]` and `[Route("api/admin/[controller]")]` or `[Route("api/web/[controller]")]`.
* **Standard Actions & Endpoints:**
  * `[HttpGet("get-all")]` -> Paginated list using `[FromQuery] FilterDto filter`.
  * `[HttpGet("{id}")]` -> Single item detail by ID.
  * `[HttpPost("upsert")]` -> Create or Update using `[FromForm]` or `[FromBody]`.
  * `[HttpPost("update-status")]` -> Status toggle using `[FromBody] UpdateStatusDto model`.
  * `[HttpPost("delete")]` -> Batch or single deletion using `[FromBody] List<int> ids`.
* Extract user context using `GetAuthenticatedUserId()`.

---

### Service Conventions (`BaseService`)
* Inherit from [`BaseService`](file:///e:/Projects/ApnaPashu/APNAPASHU_BackEnd/APNAPASHU.Service/BaseService.cs).
* Access Cloudflare R2 image uploading via `_uploader` (`R2Uploader`).
* Send HTML emails via `SendEmailAsync(EmailModel model)`.
* Always wrap operation results in `JsonModel<T>` with appropriate HTTP status codes (200 OK, 400 BadRequest, 404 NotFound).

---

### Repository Conventions (`BaseRepository`)
* Inherit from [`BaseRepository`](file:///e:/Projects/ApnaPashu/APNAPASHU_BackEnd/APNAPASHU.Repository/BaseRepository.cs).
* Database access is executed via **Dapper** using `SqlConnection` for queries/upserts (`GetAsyncList`, `GetFirstOrDefaultAsync`, `AddAsync`, `UpdateAsync`).
* **Status Updates (`UpdateStatusAsync`):** MUST use **EF Core LINQ / `AppDbContext`** (`await _context.<DbSet>.FirstOrDefaultAsync(x => x.Id == model.Id)` or `UserId == model.Id`) instead of stored procedures.
* Connections strings are loaded from `DatabaseSettings:ConnectionString` with fallback to `ConnectionStrings:DefaultConnection`.
* Use generic helper methods:
  * `GetFirstOrDefaultAsync<T>(sql, parameters, commandType)`
  * `GetAsyncList<T>(sql, parameters, commandType)`
  * `QueryMultipleAsync<T1, T2>(sql, parameters, commandType)` for multi-result set stored procedures.
  * `AddAsync(sql, parameters, commandType)` / `UpdateAsync<T>(sql, parameters, commandType)`
  * `AddBulkAsyncWithResponse<T>(sql, dataTable, parameterName)` for table-valued parameter bulk inserts.

---

### Dependency Injection Registration (`ServiceExtensions.cs`)
All new services and repositories MUST be registered in [`ServiceExtensions.cs`](file:///e:/Projects/ApnaPashu/APNAPASHU_BackEnd/APNAPASHU/Extensions/ServiceExtensions.cs):

```csharp
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<ICategoryService, CategoryService>();
```

---

## 3. Frontend Technical Standards & Conventions

### Framework & Key Packages
* **Angular Version:** `21.0.6` (Standalone Components)
* **Tailwind CSS:** `4.1.18` via `@tailwindcss/postcss`
* **Real-time Communication:** `@microsoft/signalr` (v8.0.0)
* **UI Controls & Utilities:** ApexCharts (`ng-apexcharts`), Flatpickr (`flatpickr`), FullCalendar (`@fullcalendar/angular`), Swiper (`swiper`).

---

### Base CRUD Service (`BaseCrudService`)
All feature services communicating with standard CRUD endpoints MUST extend [`BaseCrudService`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/shared/services/base-crud.service.ts):

```typescript
export class CategoryService extends BaseCrudService {
  constructor(http: HttpClient) {
    super(http, 'admin/Category');
  }
}
```

#### Provided Base Methods:
* `getAll(filter: any): Observable<any>` -> `GET api/admin/Category/get-all`
* `getById(id: number): Observable<any>` -> `GET api/admin/Category/{id}`
* `upsert(payload: any): Observable<any>` -> `POST api/admin/Category/upsert`
* `updateStatus(id: number, isActive: boolean): Observable<any>` -> `POST api/admin/Category/update-status`
* `delete(ids: number[]): Observable<any>` -> `POST api/admin/Category/delete`

---

### Master Dropdowns Service (`MasterDropDownsService`)
Do NOT write duplicate dropdown endpoints in feature services. Consume shared dropdown methods from [`MasterDropDownsService`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/shared/services/master-drop-downs.service.ts):
* `getCategoriesDropdowns()` -> Returns `List<MasterDropdownsModels>` (`id`, `name`).
* `getRolesDropdowns()` -> Returns `List<MasterDropdownsModels>` (`id`, `name`).

---

### Form Validation & Custom Validators Standard (`CustomValidators`)
Located at [`src/app/shared/validator/validators.ts`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/shared/validator/validators.ts).
All forms MUST enforce proper field validations using `CustomValidators`:
* **Email:** `[Validators.required, Validators.maxLength(250), CustomValidators.emailValidator()]`
* **Phone:** `[Validators.maxLength(50), CustomValidators.phoneValidator()]`
* **Password:** `[Validators.required, CustomValidators.passwordValidator()]`
* **Text Inputs:** Always add `Validators.maxLength(...)` matching database constraints.
* **Password Eye Icon Toggle:** All password input fields MUST feature an eye icon toggle (`showPassword` state signal/boolean with `(click)="togglePasswordVisibility()"`).
* **Required Asterisk UI Standard:** Use `<span class="text-error-500">*</span>` inside `<app-label>`.
* **Validation Error UI Standard:** Render subtext inside `<div class="mt-1 text-sm text-meta-1"><span class="text-error-500">Error message</span></div>`.

---

### Toast Notification Standard (`ToasterService`)
Do NOT create custom toast alerts. Inject [`ToasterService`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/shared/services/toaster.service.ts) from `shared/services/toaster.service`:

```typescript
private toasterService = inject(ToasterService);

// Methods:
this.toasterService.showSuccess('Category saved successfully');
this.toasterService.showError('Failed to delete selected item');
this.toasterService.showInfo('Information update');
this.toasterService.showWarning('Please fill all required fields');
```

---

### Shared Reusable Data Table (`DataTableComponent`)
Located at [`src/app/shared/components/data-table/data-table.component.ts`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/shared/components/data-table/data-table.component.ts).
Provides standard data grids with pagination, column sorting, search keyword filtering, active status toggling, and action buttons (Edit/Delete).

---

### Shared Reusable Modal Wrapper (`ModalComponent`)
Located at [`src/app/shared/components/modal/modal.component.html`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/app/shared/components/modal/modal.component.html) & `modal.component.ts`.
Provides backdrop overlay, title bar, close buttons, and responsive body container for dialog forms.
* **Standard Usage Rule:** Always pass `className="w-full max-w-[650px] p-6 lg:p-10"` and bind `(close)="closeModal()"`.
* **Standard Inner Template:**
  ```html
  <app-modal [isOpen]="isOpen" (close)="closeModal()" className="w-full max-w-[650px] p-6 lg:p-10">
    <div class="mb-6 flex flex-col justify-between sm:flex-row sm:items-center">
      <div>
        <h3 class="text-xl font-semibold text-gray-800 dark:text-white/90">Title</h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">Subtitle description</p>
      </div>
    </div>
    <form ...>
      ...
      <div class="mt-8 flex justify-end gap-3">
        <app-button type="button" variant="outline" (btnClick)="closeModal()">Cancel</app-button>
        <app-button type="submit" variant="primary">Save</app-button>
      </div>
    </form>
  </app-modal>
  ```

---

## 4. UI/UX, Design & Theme System

### Design Theme
Modern, vibrant, high-contrast dashboard & marketplace interface inspired by TailAdmin. Includes complete **Dark Mode** support configured via `@custom-variant dark (&:is(.dark *));`.

### Theme Design Tokens ([`src/styles.css`](file:///e:/Projects/ApnaPashu/APNAPASHU_FrontEnd/src/styles.css))
* **Primary Fonts:** Google Fonts `Outfit` (sans-serif) & `Nunito`.
* **Primary Brand Color Palette:**
  * Brand Base: `--color-brand-500: #465fff`
  * Brand Dark: `--color-brand-700: #2a31d8`, `--color-brand-900: #262e89`
  * Light Accent: `--color-blue-light-500: #0ba5ec`
  * Orange Warning / Highlight: `--color-orange-500: #fb6514`
  * Dark Surface: `--color-gray-900: #101828`, `--color-gray-dark: #1a2231`
  * Light Background: `--color-gray-50: #f9fafb`, White: `#ffffff`

---

## 5. Reusable Components & Methods Inventory

Before building any new class, helper, or UI element, consult this inventory:

| Component / Utility | Location | Purpose & Usage |
| :--- | :--- | :--- |
| `BaseController` | Backend API Layer | Base API controller providing `GetAuthenticatedUserId()`, `GetClientIpAddress()`, `IsValidEmail()`, `IsValidPhoneNumber()`. |
| `BaseService` | Backend Service Layer | Base service providing `_uploader` (Cloudflare R2), `SendEmailAsync()`, and `Configuration` access. |
| `BaseRepository` | Backend Repository Layer | Core Dapper generic data access (`GetFirstOrDefaultAsync`, `GetAsyncList`, `QueryMultipleAsync`, `AddAsync`, `AddBulkAsyncWithResponse`). |
| `JsonModel<T>` | Backend DataContract Layer | Standardized API response wrapper (`Data`, `Message`, `StatusCode`, `AppError`). |
| `FilterDto` | Backend DataContract Layer | Standard pagination & filter payload (`PageNumber`, `PageSize`, `SearchTerm`, `SortCulumn`, `SortDirection`). |
| `BaseCrudService` | Frontend Shared Service | Base Angular service providing `getAll`, `getById`, `upsert`, `updateStatus`, `delete`. |
| `AuthService` | Frontend Core Service | Authentication state signals (`currentUser`, `isLoggedIn`, `actualRole`, `userRole`), `isStaffRole` computed signal, `login()`, `logout()`, `toggleRole()`. |
| `ToasterService` | Frontend Shared Service | App-wide toast notification dispatcher (`showSuccess`, `showError`, `showInfo`, `showWarning`). |
| `DataTableComponent` | Frontend Shared Component | Reusable tabular data grid component with sorting, pagination, and action slots. |
| `ModalComponent` | Frontend Shared Component | Reusable dialog popup modal frame. |
| `PetCardComponent` | Frontend Shared Component | Card component displaying animal listing image, title, price, location, and quick actions. |

---

## 6. Coding & Naming Conventions

### Backend (C# / .NET)
* **Classes & Interfaces:** `PascalCase` (`CategoryController`, `ICategoryService`). Interfaces MUST be prefixed with `I`.
* **Methods & Properties:** `PascalCase` (`GetAllAsync`, `GetAuthenticatedUserId`, `StatusCode`).
* **Parameters & Local Variables:** `camelCase` (`categoryId`, `filterDto`).
* **Private Fields:** `_camelCase` (`_categoryService`, `_uploader`).
* **Async Methods:** Always suffixed with `Async` (`UpsertAsync`, `DeleteAsync`). Must return `Task` or `Task<T>`.

### Frontend (TypeScript / Angular)
* **Components, Services, Modules:** `PascalCase` (`CategoryListingComponent`, `CategoryService`).
* **File Names:** `kebab-case` with standard Angular suffixes:
  * `category-listing.component.ts`
  * `category.service.ts`
  * `auth.guard.ts`
* **Properties & Methods:** `camelCase` (`isLoggedIn`, `toggleRole()`).
* **Signals:** Use signal syntax `signal<T>(initialValue)` and read values using function execution `this.userRole()`.

---

## 7. Strict Development Rules

All future development MUST adhere to the following non-negotiable rules:

1. **Reuse Existing Components & Services:** Check `MEMORY.md` and the existing codebase before creating any new component, service, or helper method. Always reuse `DataTableComponent`, `ModalComponent`, `ToasterService`, `BaseCrudService`, `BaseController`, and `BaseRepository`.
2. **Zero Code Duplication:** Do NOT duplicate logic for data tables, modals, toast alerts, API response parsing, or CRUD HTTP calls.
3. **Preserve Architecture & Layering:** Maintain strict isolation between Controller -> Service -> Repository -> DataContract. Do NOT make database calls directly inside Controllers or Services.
4. **Follow Design Theme Tokens:** Do NOT introduce random arbitrary styling colors or raw inline styling. Use established CSS custom properties (`--color-brand-500`, `--color-gray-900`) and Tailwind classes.
5. **No Breaking Changes to Contracts:** Do NOT modify existing API signatures or response models (`JsonModel<T>`) without ensuring all consumer frontend services are updated simultaneously.
6. **Keep Viewport Isolation Intact:** Preserve `authGuard` checks and role signals (`actualRole` vs `userRole`) so buyers, sellers, and admin staff remain strictly scoped to their respective viewports.
7. **Smallest Viable Edit:** Make precise, targeted changes. Do NOT rewrite working code or re-architect existing modules unnecessarily.

---

## 8. Important Architectural Decisions

| Decision | Implementation Detail | Rationale |
| :--- | :--- | :--- |
| **HTTP-Only Cookies for JWT** | `AuthToken` cookie read by backend JWT events and set via credentials. | Protects authentication tokens against XSS attacks. |
| **Dapper for Data Access** | Handled in `BaseRepository` with `SqlConnection`. | Provides maximum query execution speed and flexibility for stored procedures. |
| **Angular Signals for State** | Used in `AuthService` (`currentUser`, `isLoggedIn`). | Enables granular reactivity without RxJS subscription memory leaks. |
| **Dual-Role Viewport Toggling** | `sellerbuyerboth` mapped dynamically in `AuthService.toggleRole()`. | Allows users with dual permissions to switch viewports without re-authenticating. |

---

## 9. Unknown / Unclear Information (TODO / Needs Confirmation)

* **TODO / NEEDS CONFIRMATION:** Complete catalog of database Stored Procedure naming standards across legacy database migrations.
* **TODO / NEEDS CONFIRMATION:** Production CDN bucket credentials for Cloudflare R2 live environment.

---

## 10. Future Task Protocol

Whenever receiving a new task:
1. **Read & Consult MEMORY.md** to review established guidelines.
2. **Inspect Existing Workspace Files** to verify existing implementations before writing new code.
3. **Reuse Existing Patterns & Tools** (`BaseCrudService`, `ToasterService`, `DataTableComponent`, `BaseRepository`, `JsonModel<T>`).
4. **Update MEMORY.md** whenever a new reusable component, permanent design pattern, or core architecture decision is introduced.
