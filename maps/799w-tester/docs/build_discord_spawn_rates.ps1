Add-Type -AssemblyName System.Drawing

$outputPath = Join-Path $PSScriptRoot 'spawn-rates-discord.png'
$width = 1200
$height = 430
$bitmap = [System.Drawing.Bitmap]::new($width, $height)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
$graphics.Clear([System.Drawing.ColorTranslator]::FromHtml('#101318'))

function New-Brush([string]$color) {
    return [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($color))
}

function New-Pen([string]$color, [float]$size = 1) {
    return [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml($color), $size)
}

function Draw-Text([string]$text, [float]$x, [float]$y, [System.Drawing.Font]$font, [System.Drawing.Brush]$brush) {
    $graphics.DrawString($text, $font, $brush, [System.Drawing.PointF]::new($x, $y))
}

function Draw-Card {
    param(
        [float]$X,
        [float]$Width,
        [string]$Accent,
        [string]$Rate
    )

    $cardBrush = New-Brush '#181D24'
    $borderPen = New-Pen '#343C47'
    $accentBrush = New-Brush $Accent
    $graphics.FillRectangle($cardBrush, $X, 84, $Width, 310)
    $graphics.DrawRectangle($borderPen, $X, 84, $Width, 310)
    $graphics.FillRectangle($accentBrush, $X, 84, $Width, 6)
    Draw-Text 'EVERY' ($X + 18) 104 $script:labelFont $script:mutedBrush
    Draw-Text $Rate ($X + 18) 121 $script:rateFont $accentBrush
    $graphics.DrawLine($borderPen, $X + 16, 160, $X + $Width - 16, 160)

    $cardBrush.Dispose()
    $borderPen.Dispose()
    $accentBrush.Dispose()
}

function Draw-UnitRow {
    param(
        [float]$X,
        [float]$Y,
        [float]$Width,
        [string]$Name,
        [string]$Accent,
        [bool]$Alternate
    )

    if ($Alternate) {
        $rowBrush = New-Brush '#1D232B'
        $graphics.FillRectangle($rowBrush, $X, $Y, $Width, 23)
        $rowBrush.Dispose()
    }

    $numberBrush = New-Brush $Accent
    $graphics.FillRectangle($numberBrush, $X + 6, $Y + 8, 8, 8)
    Draw-Text $Name ($X + 25) ($Y + 2) $script:unitFont $script:textBrush

    $numberBrush.Dispose()
}

$titleFont = [System.Drawing.Font]::new('Bahnschrift', 24, [System.Drawing.FontStyle]::Bold)
$subtitleFont = [System.Drawing.Font]::new('Segoe UI', 10, [System.Drawing.FontStyle]::Regular)
$labelFont = [System.Drawing.Font]::new('Segoe UI', 9, [System.Drawing.FontStyle]::Bold)
$rateFont = [System.Drawing.Font]::new('Bahnschrift', 25, [System.Drawing.FontStyle]::Bold)
$stateFont = [System.Drawing.Font]::new('Segoe UI', 9, [System.Drawing.FontStyle]::Bold)
$unitFont = [System.Drawing.Font]::new('Segoe UI', 10, [System.Drawing.FontStyle]::Regular)
$footerFont = [System.Drawing.Font]::new('Segoe UI', 9, [System.Drawing.FontStyle]::Regular)

$textBrush = New-Brush '#F4F6F8'
$mutedBrush = New-Brush '#9AA5B1'
$footerBrush = New-Brush '#BBC3CC'

Draw-Text 'UNIT SPAWN RATES' 24 16 $titleFont $textBrush
Draw-Text '26 unique unit types' 26 51 $subtitleFont $mutedBrush

Draw-Card -X 24 -Width 300 -Accent '#F2B84B' -Rate '6.25 SEC'
Draw-Card -X 340 -Width 520 -Accent '#45D6A8' -Rate '8.00 SEC'
Draw-Card -X 876 -Width 300 -Accent '#62A7FF' -Rate '8.75 SEC'

$eightSecond = @(
    @{ Name = 'Footmen' },
    @{ Name = 'Knight' },
    @{ Name = 'Dark Knight' },
    @{ Name = 'Archer' },
    @{ Name = 'Dryad' },
    @{ Name = 'Dark Dryad' },
    @{ Name = 'Raider' },
    @{ Name = 'Abomination' },
    @{ Name = 'Dark Abomination' },
    @{ Name = 'Banshee' },
    @{ Name = 'Warlock' },
    @{ Name = 'Mortar Bomber' },
    @{ Name = 'Dark Mortar Bomber' },
    @{ Name = 'Crypt Fiend' }
)

$sixSecond = @(
    @{ Name = 'Spellbreaker' },
    @{ Name = 'Druid of the Talon' },
    @{ Name = 'Berserker' },
    @{ Name = 'Burning Archer' },
    @{ Name = 'Militia' },
    @{ Name = 'Skeletal Mage' },
    @{ Name = 'Wraith' }
)

$eightPointSevenFive = @(
    @{ Name = 'Rifleman' },
    @{ Name = 'Huntress' },
    @{ Name = 'Headhunter' },
    @{ Name = 'Tauren' },
    @{ Name = 'Dark Tauren' }
)

for ($i = 0; $i -lt $eightSecond.Count; $i++) {
    $column = [math]::Floor($i / 7)
    $row = $i % 7
    $entry = $eightSecond[$i]
    Draw-UnitRow -X (358 + ($column * 244)) -Y (172 + ($row * 29)) -Width 226 -Name $entry.Name -Accent '#45D6A8' -Alternate (($row % 2) -eq 1)
}

for ($i = 0; $i -lt $sixSecond.Count; $i++) {
    $entry = $sixSecond[$i]
    Draw-UnitRow -X 42 -Y (172 + ($i * 29)) -Width 264 -Name $entry.Name -Accent '#F2B84B' -Alternate (($i % 2) -eq 1)
}

for ($i = 0; $i -lt $eightPointSevenFive.Count; $i++) {
    $entry = $eightPointSevenFive[$i]
    Draw-UnitRow -X 894 -Y (172 + ($i * 29)) -Width 264 -Name $entry.Name -Accent '#62A7FF' -Alternate (($i % 2) -eq 1)
}

Draw-Text 'Smaller number = faster spawning. Duplicate spawn slots are omitted.' 25 402 $footerFont $footerBrush

$bitmap.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)

$footerBrush.Dispose()
$mutedBrush.Dispose()
$textBrush.Dispose()
$footerFont.Dispose()
$unitFont.Dispose()
$stateFont.Dispose()
$rateFont.Dispose()
$labelFont.Dispose()
$subtitleFont.Dispose()
$titleFont.Dispose()
$graphics.Dispose()
$bitmap.Dispose()

Write-Output $outputPath
