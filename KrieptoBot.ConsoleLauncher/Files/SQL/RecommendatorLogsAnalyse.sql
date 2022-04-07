WITH recommendatorLogs AS (
    SELECT    
		TimeStamp,
		CAST(Properties AS xml).query('(/properties/property[@key="CorrelationId"])').value('.', 'nvarchar(max)') as CorrelationId,
        CAST(Properties AS xml).query('(/properties/property[@key="Market"])').value('.', 'nvarchar(max)') as Market,
        CAST(Properties AS xml).query('(/properties/property[@key="Recommendator"])').value('.', 'nvarchar(max)') as Recommendator,
        CAST(Properties AS xml).query('(/properties/property[@key="Score"])').value('.', 'nvarchar(max)') as Score,
        CAST(Properties AS xml).query('(/properties/property[@key="MacdValue"])').value('.', 'nvarchar(max)') as MacdValue,
        CAST(Properties AS xml).query('(/properties/property[@key="PreviousValue"])').value('.', 'nvarchar(max)') as PreviousValue,
        CAST(Properties AS xml).query('(/properties/property[@key="CurrentValue"])').value('.', 'nvarchar(max)') as CurrentValue,
        CAST(Properties AS xml).query('(/properties/property[@key="RsiValue"])').value('.', 'nvarchar(max)') as RsiValue,
        CAST(Properties AS xml).query('(/properties/property[@key="Profit"])').value('.', 'nvarchar(max)') as Profit
    FROM    
        logs
),

recommendatorScore AS
(
	SELECT TimeStamp, CorrelationId, Market, 
	CASE WHEN Recommendator = '' 
		THEN 'Final score'
		ELSE Recommendator
	END recommendator, score, MacdValue, PreviousValue, CurrentValue, RsiValue, Profit from recommendatorLogs
	WHERE Score != ''
),

recommendatorValues AS
(
	SELECT TimeStamp, CorrelationId, Market, recommendator, MacdValue, PreviousValue, CurrentValue, RsiValue, Profit from recommendatorLogs
	WHERE Recommendator != '' and Score = ''
)

select score.TimeStamp, score.Market, score.recommendator, score.score, recommendatorValues.MacdValue, recommendatorValues.PreviousValue, recommendatorValues.CurrentValue, recommendatorValues.RsiValue, recommendatorValues.Profit from recommendatorScore score
join recommendatorValues recommendatorValues 
on recommendatorValues.CorrelationId = score.CorrelationId 
and recommendatorValues.Recommendator = score.recommendator
and recommendatorValues.market = score.market 
--order by score.CorrelationId
order by score.Market, score.TimeStamp