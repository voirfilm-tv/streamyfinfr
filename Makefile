export VERSION := 0.32.0.0
export GITHUB_REPO := streamyfin/jellyfin-plugin-streamyfin
export FILE := streamyfin-${VERSION}.zip

zip:
	mkdir -p ./dist
	zip -r -j "./dist/${FILE}" Jellyfin.Plugin.Streamyfin/bin/Debug/net8.0/Jellyfin.Plugin.Streamyfin.dll packages/

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
	dotnet build
  
push-manifest:
	git commit -m 'new release' manifest.json
	git push origin main

release: update-version build zip create-tag create-gh-release update-manifest push-manifest
