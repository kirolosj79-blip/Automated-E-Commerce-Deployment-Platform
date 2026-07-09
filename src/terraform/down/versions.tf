terraform {
  required_version = ">= 1.6.0"

  # This down stack intentionally reuses the up stack local state file.
  # Running `terraform destroy` from this directory destroys everything
  # currently tracked in ../up/terraform.tfstate.
  backend "local" {
    path = "../up/terraform.tfstate"
  }

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }

    random = {
      source  = "hashicorp/random"
      version = "~> 3.6"
    }
  }
}
