<div align="center">

<br/>

```
███████╗██╗  ██╗██╗██╗     ██╗      ██╗     ██╗███╗   ██╗██╗  ██╗
██╔════╝██║ ██╔╝██║██║     ██║      ██║     ██║████╗  ██║██║ ██╔╝
███████╗█████╔╝ ██║██║     ██║      ██║     ██║██╔██╗ ██║█████╔╝ 
╚════██║██╔═██╗ ██║██║     ██║      ██║     ██║██║╚██╗██║██╔═██╗ 
███████║██║  ██╗██║███████╗███████╗ ███████╗██║██║ ╚████║██║  ██╗
╚══════╝╚═╝  ╚═╝╚═╝╚══════╝╚══════╝ ╚══════╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝
```

### **Peer-to-Peer Skill Sharing Platform**
*Trade knowledge. Earn credits. Grow together.*

<br/>

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)](https://docs.microsoft.com/aspnet/core)
[![Entity Framework](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=for-the-badge&logo=nuget&logoColor=white)](https://docs.microsoft.com/ef/core)
[![MediatR](https://img.shields.io/badge/MediatR-CQRS-FF6B35?style=for-the-badge)](https://github.com/jbogard/MediatR)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![License: MIT](https://img.shields.io/badge/License-MIT-22C55E?style=for-the-badge)](LICENSE)

<br/>

> **Skill-Link** is a community-driven web platform where people exchange skills using a **virtual credit system** — no money, no barriers. Teach what you know. Learn what you need.

<br/>

[🚀 Getting Started](#-getting-started) · [🏗️ Architecture](#️-architecture) · [📡 API Reference](#-api-reference) · [🗂️ Project Structure](#️-project-structure) · [🤝 Contributing](#-contributing)

<br/>

</div>

---

## ✨ Features

<table>
<tr>
<td width="50%">

### 🎓 Skill Discovery
Browse and search a rich feed of skills offered by community members. Filter by **category**, **skill level**, **location**, and **exchange mode**. See real ratings and reviews before you commit.

</td>
<td width="50%">

### 💳 Credit Economy
Every new member starts with a welcome credit balance. **Teach a skill → earn credits. Learn a skill → spend credits.** The system auto-transfers on session completion — no manual payments.

</td>
</tr>
<tr>
<td width="50%">

### 🔄 Skill-for-Skill Trades
Prefer a direct exchange? Propose a **skill trade** — offer what you know, request what you need. Both parties confirm, the system logs it. No credits involved.

</td>
<td width="50%">

### ⭐ Trust & Reviews
After every completed session or trade, both users rate each other (1–5 stars) and leave a short review. Reputation is earned, not bought.

</td>
</tr>
<tr>
<td width="50%">

### 💬 Integrated Messaging
Once a session or trade is accepted, a private chat thread opens between participants. Attach files, coordinate details, build rapport.

</td>
<td width="50%">

### 🛡️ Admin Panel
Full moderation suite — manage users, remove inappropriate content, adjust credits for dispute resolution, and track every admin action in an immutable audit log.

</td>
</tr>
</table>

---

## 🏗️ Architecture

Skill-Link follows **Clean Architecture** with **CQRS + MediatR** — no repository pattern, EF Core DbContext is used directly in handlers.

```
┌─────────────────────────────────────────────────────────────┐
│                         API Layer                           │
│   Controllers (thin) · Middleware · Program.cs              │
│   ↓ dispatches commands/queries via MediatR                 │
├─────────────────────────────────────────────────────────────┤
│                     Application Layer                       │
│   UseCases/          Behaviors/         Interfaces/         │
│   ├── Commands       ├── Validation     ├── IFileStorage    │
│   ├── Queries        ├── Transaction    ├── IEmailService   │
│   └── DTOs           └── Logging        └── ICurrentUser   │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                           │
│   Entities · Enums · Domain Events · Exceptions             │
│   (zero external dependencies)                              │
├─────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                      │
│   Persistence/       Identity/          Services/           │
│   ├── DbContext      ├── AppUser        ├── FileStorage     │
│   ├── Configs        ├── TokenService   ├── EmailService    │
│   └── Migrations     └── CurrentUser    └── ...            │
└─────────────────────────────────────────────────────────────┘
```

### Key Design Decisions

| Decision | Choice | Rationale |
|---|---|---|
| **Pattern** | Clean Architecture + CQRS | One handler per use case, full separation of concerns |
| **Mediator** | MediatR | Decouples controllers from business logic |
| **Data Access** | EF Core direct in handlers | DbContext is already a Unit of Work — no leaky repo abstraction |
| **Auth** | ASP.NET Core Identity + JWT | Industry standard, `ApplicationUser` stays in Infrastructure |
| **Validation** | FluentValidation via pipeline | Runs before every handler, zero validation in controllers |
| **Error Handling** | `Result<T>` pattern | Explicit failures, no exception-driven control flow |
| **File Storage** | `IFileStorageService` abstraction | Swap local disk → Azure Blob → S3 with zero application changes |
| **Cross-cutting** | MediatR pipeline behaviors | Validation, transactions, logging wired once |

---

## 🗂️ Project Structure

```
SkillLink/
│
├── SkillLink.Domain/                  # Zero dependencies
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Skill.cs
│   │   ├── Session.cs
│   │   ├── SkillTrade.cs
│   │   ├── CreditTransaction.cs
│   │   ├── Review.cs
│   │   ├── Message.cs
│   │   └── Notification.cs
│   ├── Enums/
│   │   ├── SkillLevel.cs
│   │   ├── ExchangeMode.cs
│   │   ├── SessionStatus.cs
│   │   └── TradeStatus.cs
│   ├── Events/                        # Domain events
│   │   ├── SessionCompletedEvent.cs
│   │   └── TradeCompletedEvent.cs
│   └── Exceptions/
│       ├── DomainException.cs
│       └── InsufficientCreditsException.cs
│
├── SkillLink.Application/             # Depends on Domain only
│   ├── UseCases/
│   │   ├── Auth/
│   │   ├── Skills/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateSkill/
│   │   │   │   │   ├── CreateSkillCommand.cs
│   │   │   │   │   ├── CreateSkillHandler.cs
│   │   │   │   │   └── CreateSkillValidator.cs
│   │   │   │   └── ...
│   │   │   └── Queries/
│   │   │       ├── DiscoverSkills/
│   │   │       └── GetSkillById/
│   │   ├── Sessions/
│   │   ├── Trades/
│   │   ├── Credits/
│   │   ├── Reviews/
│   │   ├── Messages/
│   │   └── Admin/
│   ├── Interfaces/
│   │   ├── ISkillLinkDbContext.cs
│   │   ├── IFileStorageService.cs
│   │   ├── IEmailService.cs
│   │   └── ICurrentUserService.cs
│   ├── Behaviors/
│   │   ├── ValidationBehavior.cs
│   │   ├── TransactionBehavior.cs
│   │   └── LoggingBehavior.cs
│   └── Common/
│       ├── Result.cs
│       └── PaginatedList.cs
│
├── SkillLink.Infrastructure/          # Depends on Application + Domain
│   ├── Persistence/
│   │   ├── SkillLinkDbContext.cs
│   │   ├── Configurations/            # IEntityTypeConfiguration per entity
│   │   └── Migrations/
│   ├── Identity/
│   │   ├── ApplicationUser.cs         # extends IdentityUser
│   │   ├── TokenService.cs
│   │   └── CurrentUserService.cs
│   └── Services/
│       ├── FileStorageService.cs
│       ├── EmailService.cs
│       └── FileStorageOptions.cs
│
└── SkillLink.API/                     # Composition root
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── UsersController.cs
    │   ├── SkillsController.cs
    │   ├── SessionsController.cs
    │   ├── TradesController.cs
    │   ├── CreditsController.cs
    │   ├── ReviewsController.cs
    │   ├── MessagesController.cs
    │   ├── NotificationsController.cs
    │   └── Admin/
    ├── Middleware/
    │   └── ExceptionHandlingMiddleware.cs
    ├── Models/Requests/               # API-only models (hold IFormFile)
    └── Program.cs
```

---

## 📡 API Reference

All endpoints are prefixed with `/api`. Authenticated endpoints require `Authorization: Bearer <token>`.

<details>
<summary><b>🔐 Authentication</b> — 7 endpoints</summary>

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/auth/register` | 🔓 | Register new user, receive welcome credits |
| `POST` | `/api/auth/login` | 🔓 | Login, receive JWT + refresh token |
| `POST` | `/api/auth/refresh` | 🔓 | Exchange refresh token for new access token |
| `POST` | `/api/auth/verify-email` | 🔓 | Verify email address |
| `POST` | `/api/auth/forgot-password` | 🔓 | Request password reset email |
| `POST` | `/api/auth/reset-password` | 🔓 | Reset password with token |
| `POST` | `/api/auth/logout` | 🔒 | Revoke refresh token |

</details>

<details>
<summary><b>👤 Users & Profiles</b> — 6 endpoints</summary>

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/users/me` | 🔒 | Get own full profile |
| `PUT` | `/api/users/me` | 🔒 | Update name, bio, phone |
| `POST` | `/api/users/me/avatar` | 🔒 | Upload profile picture |
| `GET` | `/api/users/me/skills` | 🔒 | List own skill profile |
| `POST` | `/api/users/me/skills` | 🔒 | Add skill to profile |
| `DELETE` | `/api/users/me/skills/{skillId}` | 🔒 | Remove skill from profile |
| `GET` | `/api/users/{id}` | 🔓 | View public profile |
| `GET` | `/api/users/{id}/reviews` | 🔓 | Get reviews for a user |

</details>

<details>
<summary><b>🎯 Skills</b> — 5 endpoints</summary>

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/skills` | 🔓 | Discover skills (search, filter, paginate) |
| `POST` | `/api/skills` | 🔒 | Create a skill listing |
| `GET` | `/api/skills/{id}` | 🔓 | Get skill detail |
| `PUT` | `/api/skills/{id}` | 🔒 | Update own skill listing |
| `DELETE` | `/api/skills/{id}` | 🔒 | Remove skill listing |

</details>

<details>
<summary><b>📅 Sessions</b> — 7 endpoints</summary>

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/sessions` | 🔒 | Request a credit-based session |
| `GET` | `/api/sessions` | 🔒 | List own sessions |
| `GET` | `/api/sessions/{id}` | 🔒 | Get session detail |
| `POST` | `/api/sessions/{id}/accept` | 🔒 | Provider accepts request |
| `POST` | `/api/sessions/{id}/reject` | 🔒 | Provider rejects request |
| `POST` | `/api/sessions/{id}/complete` | 🔒 | Complete session, trigger credit transfer |
| `POST` | `/api/sessions/{id}/cancel` | 🔒 | Cancel session |

</details>

<details>
<summary><b>🔄 Skill Trades</b> — 7 endpoints</summary>

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/trades` | 🔒 | Propose a skill-for-skill trade |
| `GET` | `/api/trades` | 🔒 | List own trades |
| `GET` | `/api/trades/{id}` | 🔒 | Get trade detail |
| `POST` | `/api/trades/{id}/accept` | 🔒 | Accept trade proposal |
| `POST` | `/api/trades/{id}/reject` | 🔒 | Reject trade proposal |
| `POST` | `/api/trades/{id}/complete` | 🔒 | Mark trade as completed |
| `POST` | `/api/trades/{id}/cancel` | 🔒 | Cancel trade |

</details>

<details>
<summary><b>💳 Credits · ⭐ Reviews · 💬 Messages · 🔔 Notifications · 🛡️ Admin</b></summary>

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/credits/balance` | 🔒 | Get current credit balance |
| `GET` | `/api/credits/transactions` | 🔒 | Transaction history |
| `POST` | `/api/reviews` | 🔒 | Submit post-session review |
| `DELETE` | `/api/reviews/{id}` | 🛡️ | Remove review (admin) |
| `GET` | `/api/messages/conversations` | 🔒 | List conversations |
| `GET` | `/api/messages/conversations/{userId}` | 🔒 | Get message thread |
| `POST` | `/api/messages` | 🔒 | Send message with optional attachment |
| `GET` | `/api/notifications` | 🔒 | List notifications |
| `POST` | `/api/notifications/read-all` | 🔒 | Mark all as read |
| `GET` | `/api/admin/users` | 🛡️ | List all users |
| `PATCH` | `/api/admin/credits/{userId}/adjust` | 🛡️ | Adjust credits (dispute resolution) |
| `DELETE` | `/api/admin/skills/{id}` | 🛡️ | Remove skill (moderation) |
| `GET` | `/api/admin/audit-log` | 🛡️ | View admin audit log |

</details>

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (or SQL Server Express / LocalDB)
- [Git](https://git-scm.com/)

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/ziadnaser/skill-link.git
cd skill-link
```

**2. Configure your environment**
```bash
cp SkillLink.API/appsettings.example.json SkillLink.API/appsettings.Development.json
```

Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\mssqllocaldb;Database=SkillLinkDb;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-min-32-chars",
    "Issuer": "SkillLink",
    "Audience": "SkillLinkUsers",
    "ExpiryMinutes": 60,
    "RefreshExpiryDays": 7
  },
  "FileStorage": {
    "RootPath": "",
    "BaseUrl": "https://localhost:7001"
  },
  "Credits": {
    "WelcomeBonus": 10
  }
}
```

**3. Apply database migrations**
```bash
cd SkillLink.Infrastructure
dotnet ef database update --startup-project ../SkillLink.API
```

**4. Run the API**
```bash
cd SkillLink.API
dotnet run
```

**5. Open Swagger UI**
```
https://localhost:7001/swagger
```

---

## 🧱 Domain Model

```
User ──────────────────────────────────────────────────────────┐
 │                                                              │
 ├── UserSkill[]        (skills offered/wanted)                │
 │    └── Skill ──── SkillCategory                             │
 │                                                             │
 ├── Session[] (as Provider)                                   │
 ├── Session[] (as Requester)   ──── CreditTransaction[]       │
 │                                                             │
 ├── SkillTrade[] (as Offerer)                                 │
 ├── SkillTrade[] (as Receiver)                                │
 │                                                             │
 ├── Review[] (given)                                          │
 ├── Review[] (received)                                       │
 │                                                             │
 ├── Message[] (sent / received)                               │
 ├── Notification[]                                            │
 └── CreditBalance (decimal)    ◄── updated on session complete│
                                                               │
Admin ─────────────────────────────────────────── AdminAction[]┘
```

---

## 🔐 Authentication Flow

```
Client                          API                        Database
  │                              │                             │
  │  POST /auth/register         │                             │
  ├─────────────────────────────►│  Create Domain User         │
  │                              ├────────────────────────────►│
  │                              │  Create Identity User       │
  │                              ├────────────────────────────►│
  │                              │  Award welcome credits      │
  │  { userId, credits: 10 }     ├────────────────────────────►│
  │◄─────────────────────────────┤                             │
  │                              │                             │
  │  POST /auth/login            │                             │
  ├─────────────────────────────►│  Validate credentials       │
  │                              │  Build JWT (+ domainUserId) │
  │  { accessToken, refresh }    │                             │
  │◄─────────────────────────────┤                             │
  │                              │                             │
  │  GET /api/skills             │                             │
  │  Authorization: Bearer ...   │                             │
  ├─────────────────────────────►│  Extract domainUserId claim │
  │                              │  Query DbContext directly   │
  │  { skills[] }                │                             │
  │◄─────────────────────────────┤                             │
```

---

## ⚙️ Tech Stack

<table>
<tr><td><b>Runtime</b></td><td>.NET 8</td></tr>
<tr><td><b>Framework</b></td><td>ASP.NET Core Web API</td></tr>
<tr><td><b>ORM</b></td><td>Entity Framework Core 8</td></tr>
<tr><td><b>Database</b></td><td>SQL Server / PostgreSQL / MySQL</td></tr>
<tr><td><b>Auth</b></td><td>ASP.NET Core Identity + JWT Bearer</td></tr>
<tr><td><b>Mediator</b></td><td>MediatR 12</td></tr>
<tr><td><b>Validation</b></td><td>FluentValidation</td></tr>
<tr><td><b>Docs</b></td><td>Swagger / Scalar</td></tr>
<tr><td><b>Architecture</b></td><td>Clean Architecture + CQRS</td></tr>
</table>

---

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

The project is structured for easy unit testing — handlers receive `ISkillLinkDbContext` and `IFileStorageService` interfaces, both easily mockable. No repository pattern means no extra abstraction layers to stub.

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. **Fork** the repository
2. **Create** a feature branch — `git checkout -b feature/amazing-feature`
3. **Commit** your changes — `git commit -m 'feat: add amazing feature'`
4. **Push** to the branch — `git push origin feature/amazing-feature`
5. **Open** a Pull Request

Please follow [Conventional Commits](https://www.conventionalcommits.org/) for commit messages.

---

## 📄 License

Distributed under the MIT License. See [`LICENSE`](LICENSE) for details.

---

## 👨‍💻 Author

**Ziad Naser** — TM471A Final Year Project  
Arab Open University · Faculty of Computer Studies · December 2025  
Supervisor: **Dr. Ahmed Hagag**

---

<div align="center">

*Built with ❤️ and a lot of ☕*

**[⬆ Back to top](#)**

</div>
