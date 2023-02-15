.PHONY: all
## Generate sync unit tests, format, lint, and test
all: precommit


.PHONY: build
## Build project
build:
	@dotnet build


.PHONY: clean
## Remove build files
clean:
	@dotnet clean


.PHONY: clean-build
## Build project
clean-build: clean restore build


.PHONY: precommit
## Run clean-build and test as a step before committing.
precommit: clean-build test


.PHONY: restore
## Sync dependencies
restore:
	@dotnet restore


.PHONY: test
## Run unit and integration tests
test:
	@dotnet test


# See <https://gist.github.com/klmr/575726c7e05d8780505a> for explanation.
.PHONY: help
help:
	@echo "$$(tput bold)Available rules:$$(tput sgr0)";echo;sed -ne"/^## /{h;s/.*//;:d" -e"H;n;s/^## //;td" -e"s/:.*//;G;s/\\n## /---/;s/\\n/ /g;p;}" ${MAKEFILE_LIST}|LC_ALL='C' sort -f|awk -F --- -v n=$$(tput cols) -v i=19 -v a="$$(tput setaf 6)" -v z="$$(tput sgr0)" '{printf"%s%*s%s ",a,-i,$$1,z;m=split($$2,w," ");l=n-i;for(j=1;j<=m;j++){l-=length(w[j])+1;if(l<= 0){l=n-i-length(w[j])-1;printf"\n%*s ",-i," ";}printf"%s ",w[j];}printf"\n";}'|more $(shell test $(shell uname) == Darwin && echo '-Xr')
