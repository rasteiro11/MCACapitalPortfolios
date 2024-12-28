APP_NAME = mcacapitalportfolios-api
IMAGE_TAG = v1.0
IMAGE_NAME = rasteiro11/$(APP_NAME):$(IMAGE_TAG)

DOCKERFILE_PATH = Infrastructure/Docker/Dockerfile
K8S_YAML_PATH = Infrastructure/K8s/$(APP_NAME).yaml

build-and-push:
	docker build -f $(DOCKERFILE_PATH) -t $(IMAGE_NAME) .
	docker push $(IMAGE_NAME)

deploy:
	microk8s kubectl delete -f $(K8S_YAML_PATH)
	microk8s kubectl apply -f $(K8S_YAML_PATH)
