# Terraform for AWS deployment

This folder provisions the minimum AWS foundation for the current e-commerce platform:

- VPC with public and private subnets
- EKS cluster and managed node group
- ECR repositories for all microservices
- RDS PostgreSQL
- ElastiCache Redis
- Amazon MQ RabbitMQ broker
- EFS for persistent Identity key storage
- Secrets Manager entries for database and broker credentials

## Apply

```bash
terraform init
terraform fmt
terraform plan -var-file=terraform.tfvars.example
terraform apply -var-file=terraform.tfvars.example
```

## Notes

- The WebApp is kept on HTTP and can be exposed through the existing NodePort service on port 30080.
- The EKS node group is placed in public subnets so the app can be reached without adding extra ingress services.
- RDS, Redis, Amazon MQ, and EFS are placed in private subnets.
- Jenkins should use the output ECR repository URLs and EKS cluster name to build and deploy images.
- Identity keys should be mounted from the EFS file system created here.
