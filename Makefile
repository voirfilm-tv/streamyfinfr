VERSION := $(shell git describe --tags --abbrev=0 | awk -F. -v OFS=. '{ $$2 = $$2 + 1; $$3 = 0; $$4 = 0; print }')
GITHUB_REPO := streamyfin/jellyfin-plugin-streamyfin
FILE := streamyfin-${VERSION}.zip

test:
	echo ${VERSION}
  
k: test

zip:
	mkdir -p ./dist
	zip -r -j "./dist/${FILE}" Jellyfin.Plugin.Streamyfin/bin/Release/net8.0/Jellyfin.Plugin.Streamyfin.dll packages/

csum:
	md5sum "./dist/${FILE}"

create-tag:
	git tag ${VERSION}
	git push origin ${VERSION}

create-gh-release:
	gh release create ${VERSION} "./dist/${FILE}" --generate-notes --verify-tag

update-version:
	node scripts/update-version.js
  
update-manifest:
	node scripts/validate-and-update-manifest.js

build: 
	dotnet build --configuration Release
  
push-manifest:
	git commit -m 'new release: ${VERSION}' manifest.json
	git push origin main

release: update-version build zip create-tag create-gh-release update-manifest push-manifest
