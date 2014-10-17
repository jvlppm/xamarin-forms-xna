param($installPath, $toolsPath, $package, $project)

foreach ($reference in $project.Object.References)
{
    if($reference.Name -eq "MonoGame.Framework")
    {
        $reference.CopyLocal = $false;
    }
}
