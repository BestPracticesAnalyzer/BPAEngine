<?xml version="1.0" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                version="2.0">
	<!-- Apply Templates to rules (or other elements).
	     Any attributes on the Template attribute corresponding to the Template
		 attribute on an element will be copied to the element (if missing) -->
	<xsl:output indent="yes"/>
	
	<!-- if a node has a Template attribute; use with higher priority than the default -->
	<!-- only consider Rule/Template nodes -->
	<xsl:template match="Rule[@Template] | Template[@Template]" priority="2">
		<xsl:copy>
			<xsl:apply-templates select="@Name"/>
			<xsl:call-template name="templateAttribute">
				<xsl:with-param name="templateName" select="@Template" />
			</xsl:call-template>
			<!-- copy existing attributes (which will overwrite those from Template) and nodes -->
			<xsl:apply-templates select="@*[name() != 'Name' and name() != 'Template']|node()"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template name="templateAttribute">
		<xsl:param name="templateName" />
		<!-- if we weren't passed the name if a template, stop recursing-->
		<xsl:if test="$templateName">
			<!-- recurse -->
			<xsl:call-template name="templateAttribute" >
				<xsl:with-param name="templateName" select="/*/Configuration/Template[@Name=$templateName]/@Template" />
			</xsl:call-template>
			<!-- copy attributes (which will overwrite those from deeper Templates) -->
			<xsl:copy-of select="/*/Configuration/Template[@Name=$templateName]/@*[name()!='Name' and name()!='Template']" />
		</xsl:if>
	</xsl:template>
	
	<!-- by default, just copy the node -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>

