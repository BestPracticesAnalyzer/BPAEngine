<?xml version="1.0" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:exbpa="usr:exbpa-scripts"
                version="2.0">

<xsl:output indent="yes" />

<xsl:template match="/">
			<xsl:apply-templates select="ObjectCollector"/>
</xsl:template>


<xsl:template match="ObjectCollector">
	<Report>
		<xsl:apply-templates />
	</Report>
</xsl:template>

<xsl:template match="Configuration">
	<Configuration CustomerName="{@CustomerName}">
		<xsl:apply-templates select="Run" />
	</Configuration>
</xsl:template>
  
<xsl:template match="Run">
	<Run ConfigVersion="{@ConfigVersion}"
	     StartTime="{@StartTime}"
		 EndTime="{@EndTime}"
		 Operations="{@Operations}"
		 Level="{@Level}">
		<xsl:if test="@Categories != ''">
			<xsl:attribute name="Categories">
				<xsl:value-of select="@Categories" />
			</xsl:attribute>
		</xsl:if>
		<xsl:if test="@Scope != ''">
			<xsl:attribute name="Scope">
				<xsl:value-of select="substring-after(@Scope, '=')" />
			</xsl:attribute>
		</xsl:if>
		<xsl:if test="@DC != ''">
			<xsl:attribute name="DC">
				<xsl:value-of select="@DC" />
			</xsl:attribute>
		</xsl:if>
	</Run>
</xsl:template>
  
<xsl:template match="Object">
	<xsl:choose>
		<xsl:when test="@Class">
			<Object Name="{@Class}">
				<xsl:apply-templates select="*" />
			</Object>
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates select="*" />
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<xsl:template match="Instance">
	<xsl:choose>
		<xsl:when test="@Class">
			<xsl:variable name="instanceName" select="@Name" />
			<xsl:choose>
				<xsl:when test="$instanceName">
					<Instance Name="{$instanceName}">
						<xsl:apply-templates select="*" />
					</Instance>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="*" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates select="*" />
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<xsl:template match="Message">
	<Message Name="{@Name}"
	         Title="{@Title}"
	         Error="{@Error}"
	         SettingType="{../@Type}"
	         Value="{../Value}">

		<!-- get path of setting, based on Type -->
		<xsl:attribute name="SettingPath">
			<xsl:choose>
				<xsl:when test="../../@Type='Registry'">
					<xsl:value-of select="concat(../../@Key2, '\', ../../@Key3, '\', ../@Key1)" />
				</xsl:when>
				<xsl:when test="../../@Type='Directory'">
					<xsl:value-of select="concat(../../@Key2, '\', ../@Key1)" />
				</xsl:when>
				<xsl:when test="../../@Type='Perfmon'">
					<xsl:value-of select="concat(../../@Key2, '(', ../../@Key3, ')\', ../@Key1)" />
				</xsl:when>
				<xsl:when test="../../@Type='WMI'">
					<xsl:value-of select="concat(../../@Key2, '\', ../../@Key3, '\', ../@Key1)" />
				</xsl:when>
				<xsl:when test="../../@Type='Metabase'">
					<xsl:value-of select="concat(../../@Key2, '\', ../@Key1)" />
				</xsl:when>
				<xsl:when test="../../@Type='File'">
					<xsl:value-of select="concat(../../@Key1, '\', ../../@Key2, ' (', ../@Key1, ')')" />
				</xsl:when>
				<xsl:when test="../../@Type='Group'">
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="concat(../../@Type, ../../@Key1)" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
		
		
		<xsl:apply-templates select="text()" />
	</Message>
</xsl:template>


<!-- ignore and process children -->
<xsl:template match="Setting">
	<xsl:apply-templates select="*" />
</xsl:template>

<xsl:template match="Rule">
	<xsl:apply-templates select="*" />
</xsl:template>


<!-- these are just dummies to suppress printing the text under them -->
<xsl:template match="Value">
</xsl:template>

<xsl:template match="Result">
</xsl:template>

</xsl:stylesheet>


