# Terraform destroy stack

This folder is a teardown-only Terraform root.

It is configured to use the same local state file as the deployment stack:

- `../up/terraform.tfstate`

Because this root defines no resources, running `terraform destroy` here removes
all resources tracked in that state file from AWS.

## Usage

```bash
cd src/terraform/down
terraform init
terraform destroy -auto-approve
```

## Optional explicit region

```bash
terraform destroy -var aws_region=us-east-1 -auto-approve
```

## Important

- This only destroys resources that are managed by `../up/terraform.tfstate`.
- If you changed the `up` stack backend from local to remote, update the backend
  block in `versions.tf` here to point to the same backend before destroy.
