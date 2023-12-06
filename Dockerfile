FROM mcr.microsoft.com/dotnet/sdk:6.0.403

######## Installing Splunk ########
ENV SPLUNK_UF_VERSION "splunkforwarder-8.2.7.1-c2b65bc24aea-Linux-x86_64.tgz"
ENV SPLUNK_UF_DOWNLOAD_LINK "s3://tf-stz-aws45-splunk-cloud-us-east-1/forwarder/$SPLUNK_UF_VERSION"
ENV SPLUNK_CLOUD_APP_DOWNLOAD_LINK "s3://tf-stz-aws45-splunk-cloud-us-east-1/apps/splunkclouduf.spl"

WORKDIR /opt

    # make directories for published app and sources
RUN mkdir -p /app /opt/src \
	# Download and untar Splunk forwarder
	&& apt-get update && apt-get install -y --no-install-recommends unzip && rm -rf /var/lib/apt/lists/* \
	&& curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip" \
	&& unzip awscliv2.zip && ./aws/install \
	&& aws s3 cp $SPLUNK_UF_DOWNLOAD_LINK . \
	&& tar -zxvf $SPLUNK_UF_VERSION -C /opt --no-same-owner \
	&& aws s3 cp $SPLUNK_CLOUD_APP_DOWNLOAD_LINK /opt/splunkforwarder
######## Installing Splunk ########

COPY NuGet.Config /opt/NuGet.Config
COPY stylecop.ruleset /opt/stylecop.ruleset
COPY src/ /opt/src

# publish the app
RUN dotnet publish -c Release -o /app /opt/src/AuthCraft.Api

FROM mcr.microsoft.com/dotnet/aspnet:6.0.11


WORKDIR /app
# copying published app from the previous build step
COPY --from=0 /app .

EXPOSE 80/tcp

CMD ["/opt/splunkforwarder/run.sh"]
