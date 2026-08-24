# FlowPath — Workflow & Onboarding Management SaaS

**FlowPath** is a multi-tenant, subscription-based SaaS platform for building and running business workflows — most notably employee onboarding. Organisations design visual workflow templates, launch workflow instances for individual employees, and track task completion, documents, comments, issues, and audit trails from a role-based dashboard.

The platform is split into three deployable apps:

| App | Path | Purpose | Stack |
| --- | --- | --- | --- |
| Backend API | `fyp-backend/` | REST API, auth, business logic, persistence | .NET 8, ASP.NET Core Web API |
| Main web app | `fyp-frontend/` | The product UI (workflows, tasks, admin) | React 19 + TypeScript + Vite |
| Landing site | `workflow-management-saas/` | Marketing/landing page & sign-up flow | React 19 + TypeScript + Vite |

---

## Features

### Workflow management
- **Visual workflow builder** — drag-and-drop editor for designing workflow templates (`@xyflow/react`).
- **Workflow templates & nodes** — reusable templates composed of nodes with task dependencies.
- **Workflow instances** — launch a template against an employee/account and track progress end-to-end.

### Task types
Each workflow node can run one of five polymorphic task types:
- **Checklist** — ordered checklist items that must be completed.
- **Read document** — assign and acknowledge reading of policies/contracts.
- **Upload document** — collect files (forms, contracts, compliance docs).
- **Project task** — free-form project work items.
- **Feedback task** — capture structured feedback.

### Onboarding & accounts
- Account, organisation, and department management.
- Role-based dashboards for supervisors and employees (HR, managers, IT, onboarders).
- Employee onboarding details, account directory, and user invitations.

### Documents, comments & issues
- Document storage with shareable access links.
- Comment threads on workflow instances and tasks.
- Reported issues with status tracking.

### Analytics & audit
- Real-time analytics: completion rates, bottlenecks, time-to-completion.
- Full workflow instance audit trail with timestamps for compliance.

### Notifications
- In-app notification system for task assignment, completion, and updates.

### SaaS / subscription layer
- Stripe-hosted checkout and subscription management.
- Automated tenant provisioning: signing up creates an organisation and promotes the user to owner/admin.
- Per-organisation database schema isolation and automatic schema switching based on user context.
- Subscription tiering with tenant entitlement limits (workflow templates, task templates, document uploads).
- Landing page with pricing and a "get started" flow that redirects into the product.

---

## Tech stack

### Backend — `fyp-backend/`
- **.NET 8** / **ASP.NET Core Web API**
- **Entity Framework Core 8** with **Pomelo MySQL** provider
- **MediatR** — CQRS-style request/command handlers
- **AutoMapper** — DTO/entity mapping
- **JWT Bearer** authentication with custom authorization policies
- **Stripe.net** — subscriptions, checkout, webhooks
- **Swashbuckle** — Swagger/OpenAPI docs
- **ASP.NET Core Rate Limiting** — fixed-window policies for pricing and sign-up endpoints
- Layered solution architecture:
  - `OnboardingWFMSApi.Presentation` — controllers, `Program.cs`, configuration
  - `OnboardingWFMSApi.BusinessLogic` — logic, MediatR handlers, factories, Stripe/tenant services
  - `OnboardingWFMSApi.DataAccess` — `ApplicationDbContext`, repositories, infrastructure
  - `OnboardingWFMSApi.DataModels` — tables, models, DTOs, payloads, mapping profiles

### Main app — `fyp-frontend/`
- **React 19** + **TypeScript**
- **Vite** build tooling
- **Redux Toolkit** + **React Redux** — global state
- **React Router 7** — routing
- **Tailwind CSS 4** + **shadcn/ui**-style components (**Radix UI** primitives)
- **@xyflow/react (React Flow)** — visual workflow builder
- **Recharts** — analytics charts
- **Axios**, **date-fns**, **luxon**, **framer-motion**, **sonner** (toasts)

### Landing site — `workflow-management-saas/`
- **React 19** + **TypeScript** + **Vite**
- **Tailwind CSS** + **Radix UI** + **lucide-react**

### Infrastructure
- **Docker** / **Docker Compose** — MySQL, backend, frontend, and landing containers
- **MySQL 8.0** — relational store
- **Nginx** — static serving for both frontends

---

## Getting started

### Prerequisites
- [Docker](https://www.docker.com/products/docker-desktop/) with Docker Compose
- A Stripe test API key (optional, for subscriptions — see `.env`)

### Run everything with Docker

```bash
docker compose up --build
```

This starts:

| Service | URL | Notes |
| --- | --- | --- |
| MySQL | `localhost:3307` | root password `pword123`, database `onboarding-wfms-db` |
| Backend API | `http://localhost:5000` | Swagger at `http://localhost:5000/swagger` |
| Main app | `http://localhost:5173` | The product UI |
| Landing site | `http://localhost:5174` | Marketing & sign-up |

### Default tenants / login

Seeded idempotently on startup:

| Tenant | Email | Password |
| --- | --- | --- |
| `default-tenant-1` | `admin1@test.co.uk` | `pword123` |
| `default-tenant-2` | `admin2@test.co.uk` | `pword123` |

### Environment variables

Copy `.env` and fill in your Stripe keys (used by `docker-compose.yml`):

```env
STRIPE_SECRET_KEY=sk_test_...
STRIPE_WEBHOOK_SECRET=whsec_...
```

Backend configuration lives in `fyp-backend/OnboardingWFMSApi/OnboardingWFMSApi.Presentation/appsettings.json` (see `appsettings.json.sample`), including the MySQL connection string and JWT auth settings.

---

## Running locally (without Docker)

**Backend** — requires .NET 8 SDK and a MySQL instance:

```bash
cd fyp-backend/OnboardingWFMSApi
dotnet run --project OnboardingWFMSApi.Presentation
```

**Main app** — requires Node.js:

```bash
cd fyp-frontend
npm install
npm run dev
```

**Landing site**:

```bash
cd workflow-management-saas
npm install
npm run dev
```

---

## Project layout

```
workflow-management-saas/
├── docker-compose.yml          # MySQL + backend + frontend + landing
├── fyp-backend/                # .NET 8 API solution
│   └── OnboardingWFMSApi/
│       ├── OnboardingWFMSApi.Presentation/    # Controllers, Program.cs
│       ├── OnboardingWFMSApi.BusinessLogic/   # Logic & MediatR handlers
│       ├── OnboardingWFMSApi.DataAccess/      # DbContext & repositories
│       └── OnboardingWFMSApi.DataModels/      # Tables, DTOs, payloads
├── fyp-frontend/               # Main React product app
│   └── src/
│       ├── app/                # Layout, pages, dialogs
│       ├── components/         # Shared UI components
│       ├── features/           # Redux slices
│       ├── models/             # TypeScript domain models
│       └── hooks/              # Custom React hooks
└── workflow-management-saas/   # Landing / marketing site
    └── src/app/pages/          # LandingPage, GetStartedPage
```
