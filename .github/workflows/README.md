# GitHub Actions Workflows for Mortgage Microservices

This folder contains GitHub Actions workflows for deploying all mortgage microservices to OpenShift on AWS (ROSA).

## Directory Structure

```
github-workflows/
├── customer-service/.github/workflows/
│   ├── 1-build-push-ecr.yml
│   ├── 2-deploy-openshift.yml
│   ├── 3-verify-health.yml
│   ├── 4-invoke-apis.yml
│   └── 5-remove-openshift.yml
├── property-service/.github/workflows/
│   └── (same 5 workflows)
├── loans-service/.github/workflows/
│   └── (same 5 workflows)
├── payments-service/.github/workflows/
│   └── (same 5 workflows)
└── application-service/.github/workflows/
    └── (same 5 workflows)
```

## Workflow Descriptions

| Workflow | Description |
|----------|-------------|
| **1-build-push-ecr.yml** | Build .NET Docker image and push to Amazon ECR |
| **2-deploy-openshift.yml** | Deploy service to OpenShift with ConfigMaps, Deployments, Services, Routes |
| **3-verify-health.yml** | Verify liveness, readiness, and health endpoints |
| **4-invoke-apis.yml** | Run smoke, CRUD, and workflow API tests |
| **5-remove-openshift.yml** | Remove service from OpenShift (requires confirmation) |

## Required GitHub Secrets

Configure these secrets in each repository:

| Secret | Description |
|--------|-------------|
| `AWS_ACCESS_KEY_ID` | AWS access key with ECR push permissions |
| `AWS_SECRET_ACCESS_KEY` | AWS secret key |
| `OPENSHIFT_SERVER` | OpenShift cluster URL (e.g., `https://api.cluster.example.com:6443`) |
| `OPENSHIFT_TOKEN` | OpenShift service account token |

## Service Dependencies & Deploy Order

```
┌─────────────────────┐
│  customer-service   │ (Port 5001) - No dependencies
│  property-service   │ (Port 5002) - No dependencies
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│   loans-service     │ (Port 5003) - Depends on: customer, property
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  payments-service   │ (Port 5004) - Depends on: customer, loans
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ application-service │ (Port 5005) - Depends on: ALL services
└─────────────────────┘
```

**Deploy in this order:**
1. customer-service & property-service (can be parallel)
2. loans-service
3. payments-service
4. application-service

## Installation

For each repository, copy the `.github/workflows/` folder:

```bash
# Example for customer-service
cp -r github-workflows/customer-service/.github /path/to/mortgage-customer-service/
```

## Pre-requisites

### 1. Create ECR Repositories

```bash
aws ecr create-repository --repository-name mortgage/customer-service
aws ecr create-repository --repository-name mortgage/property-service
aws ecr create-repository --repository-name mortgage/loans-service
aws ecr create-repository --repository-name mortgage/payments-service
aws ecr create-repository --repository-name mortgage/application-service
```

### 2. Create OpenShift Namespace

```bash
oc new-project mortgage-app
```

### 3. Create OpenShift Service Account (for GitHub Actions)

```bash
# Create service account
oc create sa github-actions -n mortgage-app

# Grant permissions
oc policy add-role-to-user edit -z github-actions -n mortgage-app

# Get token
oc create token github-actions -n mortgage-app --duration=8760h
```

## Usage

### Build and Push Image

```yaml
# Trigger: Push to main or manual dispatch
# Inputs: image_tag (optional)
```

### Deploy to OpenShift

```yaml
# Trigger: Manual dispatch only
# Inputs:
#   - image_tag: Tag to deploy (required)
#   - environment: dev|staging|prod (required)
```

### Verify Health

```yaml
# Trigger: Manual dispatch
# Inputs:
#   - environment: dev|staging|prod
```

### Invoke APIs

```yaml
# Trigger: Manual dispatch
# Inputs:
#   - environment: dev|staging|prod
#   - test_type: smoke|full|crud|workflow
```

### Remove from OpenShift

```yaml
# Trigger: Manual dispatch
# Inputs:
#   - environment: dev|staging|prod
#   - confirm_delete: Must type service name to confirm
```

## Service URLs (after deployment)

| Service | Internal URL | External Route |
|---------|--------------|----------------|
| customer-service | http://customer-service.mortgage-app.svc.cluster.local | https://customer-service-mortgage-app.apps.{cluster} |
| property-service | http://property-service.mortgage-app.svc.cluster.local | https://property-service-mortgage-app.apps.{cluster} |
| loans-service | http://loans-service.mortgage-app.svc.cluster.local | https://loans-service-mortgage-app.apps.{cluster} |
| payments-service | http://payments-service.mortgage-app.svc.cluster.local | https://payments-service-mortgage-app.apps.{cluster} |
| application-service | http://application-service.mortgage-app.svc.cluster.local | https://application-service-mortgage-app.apps.{cluster} |

## Health Check Endpoints

All services expose:
- `/api/health` - Main health status
- `/api/health/live` - Liveness probe
- `/api/health/ready` - Readiness probe (includes dependency checks)

## Environment Variables

Each service ConfigMap includes:

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Environment name (dev/staging/prod) |
| `ASPNETCORE_URLS` | Service binding URL |
| `ServiceUrls__*` | URLs to dependent services |

## Troubleshooting

### View Logs
```bash
oc logs -l app=customer-service -n mortgage-app --tail=100
```

### Check Events
```bash
oc get events -n mortgage-app --sort-by='.lastTimestamp'
```

### Describe Deployment
```bash
oc describe deployment customer-service -n mortgage-app
```

### Test Service Connectivity
```bash
# From within cluster
oc run test-pod --rm -it --image=curlimages/curl -- \
  curl http://customer-service.mortgage-app.svc.cluster.local/api/health
```
