
docker build -t moimhossain/dapr-backend:v1 -f .\Dapr.Backend\Dockerfile .
docker push moimhossain/dapr-backend:v1

docker build -t moimhossain/dapr-frontend:v1 -f .\Dapr.Frontend\Dockerfile .
docker push moimhossain/dapr-frontend:v1