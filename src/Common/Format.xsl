<?xml version="1.0" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:exbpa="usr:exbpa-scripts"
                version="2.0">

<xsl:output indent="yes" />


<msxsl:script language="C#" implements-prefix="exbpa">
	<![CDATA[
			public string FormatDate(string str, string pattern)
			{
				string date = String.Empty;
				try
				{
					date = DateTime.Parse(str, System.Globalization.CultureInfo.InvariantCulture).ToString(pattern);
				}
				catch
				{
					date = DateTime.MinValue.ToString(pattern);
				}
				return date;
			}
			public double NumifyLevel(string level)
			{
				switch (level)
				{
					case "Error":
						return 0;
						break;
					case "Warning":
						return 1;
						break;
					case "NonDefault":
						return 2;
						break;
					case "Time":
						return 3;
						break;
					case "None":
						return 4;
						break;
					default:
						return 5;
						break;
				}
			}
	]]>
</msxsl:script>

<xsl:template match="/">
	<html>
		<head>
			<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
			<title>Exchange Server Best Practices Analyzer Error Report</title>
			<style type="text/css">
				div.linkLevel { font-weight: bold; font-size: 120% }
				div.linkObject { font-weight: bold }
				div.linkInstance {  }
				span.linkInstance {  }
				
				div.sectionLevel { font-weight: bold; font-size: 135% }
				div.sectionObject { font-weight: bold; font-size: 120% }
				div.sectionInstance { font-weight: bold }
				
				div.indentObject { margin: 0px 0px 0px 20px; border-left: thin solid gray; border-top: thin solid gray }
				div.indentInstance { margin: 0px 0px 0px 20px }
				
				div.indent { margin: 0px 0px 0px 20px }
				span.indent { margin: 0px 0px 0px 10px }
				div.setting { font-size: 80% }
				
				*.Error { color: red }
				*.Warning { color: darkOrange }
				*.NonDefault { color: brown }
				*.Time { color: blue }
				*.None { color: darkGreen }

				body { font-family: Ariel, sans-serif; font-size: 80%  }
				table { font-size: 100%; border-collapse: collapse }
				th, td { padding: 1px 4px }
			</style>
		</head>
		
		<body>
			<!-- header info -->
			<xsl:apply-templates select="Report"/>

			<!-- summary links -->
			<p>
				<xsl:call-template name="Links">
					<xsl:with-param name="title" select="'Errors'" />
					<xsl:with-param name="level" select="'Error'" />
				</xsl:call-template>
				<xsl:call-template name="Links">
					<xsl:with-param name="title" select="'Warnings'" />
					<xsl:with-param name="level" select="'Warning'" />
				</xsl:call-template>
				<xsl:call-template name="Links">
					<xsl:with-param name="title" select="'Non-default settings'" />
					<xsl:with-param name="level" select="'NonDefault'" />
				</xsl:call-template>
				<xsl:call-template name="Links">
					<xsl:with-param name="title" select="'Recent changes'" />
					<xsl:with-param name="level" select="'Time'" />
				</xsl:call-template>
				<xsl:call-template name="Links">
					<xsl:with-param name="title" select="'Other Items of Interest'" />
					<xsl:with-param name="level" select="'None'" />
				</xsl:call-template>
			</p>
			<!-- main body of report -->
			<p>
				<xsl:call-template name="Section">
					<xsl:with-param name="title" select="'Error Details'" />
					<xsl:with-param name="level" select="'Error'" />
				</xsl:call-template>
				<xsl:call-template name="Section">
					<xsl:with-param name="title" select="'Warning Details'" />
					<xsl:with-param name="level" select="'Warning'" />
				</xsl:call-template>
				<xsl:call-template name="Section">
					<xsl:with-param name="title" select="'Settings changed from default values'" />
					<xsl:with-param name="level" select="'NonDefault'" />
				</xsl:call-template>
				<xsl:call-template name="Section">
					<xsl:with-param name="title" select="'Recent changes'" />
					<xsl:with-param name="level" select="'Time'" />
				</xsl:call-template>
				<xsl:call-template name="Section">
					<xsl:with-param name="title" select="'Other Items of Interest'" />
					<xsl:with-param name="level" select="'None'" />
				</xsl:call-template>
			</p>
		
			<h3>Listing by Rules</h3>
			<xsl:for-each select="/*/Configuration/Rules/Rule" >
				<xsl:sort select="exbpa:NumifyLevel(@Error)" />
				<div class="linkLevel">
					<div class="{@Error}">
						<xsl:choose>
							<xsl:when test="@Title != ''">
								<xsl:value-of select="@Title" />
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="@Name" />
							</xsl:otherwise>
						</xsl:choose>
					</div>
				</div>
				<xsl:apply-templates select="/*/Object | /*/Message" mode="rule">
					<xsl:with-param name="ruleName" select="@Name" />
				</xsl:apply-templates>
			</xsl:for-each>

		</body>
	</html>
</xsl:template>
  
  
<!-- title and table of Run elements -->
<xsl:template match="Report">
	<h1>Exchange Server Best Practices Analyzer</h1><h2>Error Report<xsl:if test="Configuration/@CustomerName != ''"> for <xsl:value-of select="Configuration/@CustomerName" /></xsl:if></h2>
	<xsl:if test="count(.//Message) = 0">
		WARNING: No messages in file.  Analysis may not have been run.<br />
	</xsl:if>
	
	<!-- the table of Run elements -->
	<div class="indent">
		<table >
			<tr>
				<th>Time</th>
				<th />
				<th>Operations</th>
				<th>Configuration</th>
				<th>Restrictions</th>
			</tr>
			<xsl:for-each select="Configuration/Run">
				<tr>
					<td style="border-top: thin solid gray" valign="top">
						<table>
							<tr>
								<th>start</th>
								<td>
									<xsl:value-of select="exbpa:FormatDate(@StartTime, 'MMMM d, yyyy HH:mm:ss')" />
								</td>
							</tr>
							<tr>
								<th>end</th>
								<td>
									<xsl:value-of select="exbpa:FormatDate(@EndTime, 'MMMM d, yyyy HH:mm:ss')" />
								</td>
							</tr>
						</table>
					</td>
					<td style="border-top: thin solid gray"></td>
					<td style="border-top: thin solid gray" valign="top"><xsl:value-of select="@Operations" /></td>
					<td style="border-top: thin solid gray" valign="top"><xsl:value-of select="@ConfigVersion" /></td>
					<td style="border-top: thin solid gray" valign="top">
						<table>
							<xsl:if test="boolean(@Level)">
								<tr><td>
									Level: <xsl:value-of select="@Level" />
								</td></tr>
							</xsl:if>
							<xsl:if test="boolean(@Scope)">
								<tr><td>
									Scope: <xsl:value-of select="@Scope" />
								</td></tr>
							</xsl:if>
							<xsl:if test="boolean(@Categories)">
								<tr><td>
									Categories: <xsl:value-of select="@Categories" />
								</td></tr>
							</xsl:if>
						</table>
					</td>
				</tr>
			</xsl:for-each>
		</table>
	</div>
</xsl:template>




<!-- the links templates handles creating the links at the top of the report -->
<!-- do one of these for each Error Level -->
<xsl:template name="Links">
	<xsl:param name="title" />
	<xsl:param name="level" />

	<xsl:variable name="count" select="count(.//Message[@Error=$level])" />
	<div class="linkLevel">
		<a href="#{$level}"><xsl:value-of select="concat($title, ' (', $count, ')')" /></a>
	</div>
	<xsl:apply-templates select="/*/Object" mode="links">
		<xsl:with-param name="anchor" select="$level" />
		<xsl:with-param name="level" select="$level" />
	</xsl:apply-templates>
</xsl:template>

<xsl:template match="Object|Instance" mode="links">
	<xsl:param name="anchor" />
	<xsl:param name="level" />
	<xsl:variable name="count" select="count(.//Message[@Error=$level])" />
	<xsl:if test="$count">
		<div class="indent">
			<xsl:variable name="newAnchor" select="concat($anchor, '_', @Name)" />
			<div class="{concat('link', local-name())}">
				<a href="#{$newAnchor}"><xsl:value-of select="concat(@Name, ' (', $count, ')')" /></a>
			</div>
			<xsl:apply-templates select="Object|Instance" mode="links">
				<xsl:with-param name="anchor" select="$newAnchor" />
				<xsl:with-param name="level" select="$level" />
			</xsl:apply-templates>
		</div>
	</xsl:if>
</xsl:template> 



<!-- do one of these for each Error Level -->
<xsl:template name="Section">
	<xsl:param name="title" />
	<xsl:param name="level" />
	
	<div class="{$level}">
		<div class="sectionLevel">
			<a name="{$level}" /><xsl:value-of select="$title" />
		</div>
	</div>

	<xsl:apply-templates select="*/Object | */Instance | */Message">
		<xsl:with-param name="level" select="$level" />
		<xsl:with-param name="anchor" select="$level" />
	</xsl:apply-templates>
</xsl:template>

<!-- the standard templates create most of the report -->
<xsl:template match="Object|Instance">
	<xsl:param name="anchor" />
	<xsl:param name="level" />
	<xsl:if test=".//Message[@Error=$level]">
		<xsl:variable name="newAnchor" select="concat($anchor, '_', @Name)" />
		<a name="{$newAnchor}" />
		<div class="{concat('section', local-name())}">
			<xsl:value-of select="@Name" />
		</div>
		<div class="{concat('indent', local-name())}">
			<xsl:apply-templates select="*">
				<xsl:with-param name="level" select="$level" />
				<xsl:with-param name="anchor" select="$newAnchor" />
			</xsl:apply-templates>
		</div>
	</xsl:if>
</xsl:template>

<xsl:template match="Message">
	<xsl:param name="level" />
	<!-- only process for matching error level -->
	<xsl:if test="@Error = $level">
		<div class="{$level}">
			<xsl:apply-templates select="node()"/>
		</div>
		<div class="setting">
			<xsl:if test="@SettingPath != ''">
				Setting: <xsl:value-of select="@SettingPath" /><br />
			</xsl:if>
			<xsl:if test="@Value != ''">
				Value: <xsl:value-of select="@Value" /><br />
			</xsl:if>
		</div>
	</xsl:if>
</xsl:template>



<!-- follow this for each (distinct) ruleName -->
<xsl:template match="Object|Instance" mode="rule">
	<xsl:param name="ruleName" />

	<xsl:if test=".//Message[@Name=$ruleName]">
		<div class="indent">
			<xsl:choose>
				<xsl:when test="local-name() = 'Object'">
					<div class="{concat('link', local-name())}">
						<xsl:value-of select="@Name" />
					</div>
				</xsl:when>
				<xsl:otherwise>
					<span class="{concat('link', local-name())}">
						<xsl:value-of select="@Name" />
					</span>
				</xsl:otherwise>
			</xsl:choose>
			
			<xsl:apply-templates select="Object|Instance|Message" mode="rule">
				<xsl:with-param name="ruleName" select="$ruleName" />
			</xsl:apply-templates>
		</div>
	</xsl:if>
</xsl:template> 

<xsl:template match="Message" mode="rule">
	<xsl:param name="ruleName" />
	<xsl:if test="@Name=$ruleName">
		<span class="indent">
			<span class="{@Error}">
				<xsl:apply-templates select="node()"/>
			</span>
		</span>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>


