export VERSION := $(shell git describe --tags --abbrev=0 | awk -F. -v OFS=. '{ $$2 = $$2 + 1; $$3 = 0; $$4 = 0; print }')
export GITHUB_REPO := streamyfin/jellyfin-plugin-streamyfin
export FILE := streamyfin-${VERSION}.zip

print:
	echo ${VERSION}

k: zip

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
	sed -i 's/\(.*\)<\(.*\)Version>\(.*\)<\/\(.*\)Version>/\1<\2Version>${VERSION}<\/\4Version>/g' ./Jellyfin.Plugin.Streamyfin/Jellyfin.Plugin.Streamyfin.csproj
  
update-manifest:
	node scripts/validate-and-update-manifest.js

test: 
	dotnet test Jellyfin.PLugin.Streamyfin.Tests

build: 
	dotnet build Jellyfin.PLugin.Streamyfin --configuration Release
  
push-manifest:
	git commit -m 'new release: ${VERSION}' manifest.json
	git push origin main

release: print update-version build zip create-tag create-gh-release update-manifest push-manifest
