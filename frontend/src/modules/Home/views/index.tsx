import { Button, Stack } from 'mada-design-system'
import { Link } from 'react-router-dom'

const index = () => {
    return (
        <div className="min-h-screen">
            {/* Hero Section */}
            <Stack 
                justifyContent={'center'} 
                alignItems={'center'} 
                className="min-h-[70vh] bg-gradient-to-br from-primary/10 to-primary/5 px-4"
            >
                <Stack direction="col" alignItems="center" gap={6} className="max-w-4xl text-center">
                    <h1 className="display-01 font-bold text-foreground">
                        Ikhtibar Educational Portal
                    </h1>
                    <p className="text-body-01 text-foreground-secondary max-w-2xl">
                        Comprehensive exam management system for educational institutions
                    </p>
                    <Stack gap={4} className="mt-8">
                        <Button asChild variant="default" colors="primary" size="default">
                            <Link to="/support">
                                Support
                            </Link>
                        </Button>
                        <Button asChild variant="outline" colors="primary" size="default">
                            <Link to="/login">
                                Login
                            </Link>
                        </Button>
                    </Stack>
                </Stack>
            </Stack>

            {/* Services Section */}
            <Stack 
                justifyContent={'center'} 
                alignItems={'center'} 
                className="py-16 px-4"
            >
                <Stack direction="col" alignItems="center" gap={8} className="max-w-6xl">
                    <h2 className="title-01 font-bold text-foreground">Available Services</h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 w-full">
                        <div className="bg-card rounded-lg p-6 text-center hover:shadow-lg transition-shadow">
                            <h3 className="text-body-01 font-semibold mb-2">Support Services</h3>
                            <p className="text-body-02 text-foreground-secondary mb-4">Get help and assistance</p>
                            <Button asChild variant="outline" size="sm">
                                <Link to="/support">Access</Link>
                            </Button>
                        </div>
                        <div className="bg-card rounded-lg p-6 text-center hover:shadow-lg transition-shadow">
                            <h3 className="text-body-01 font-semibold mb-2">FAQ</h3>
                            <p className="text-body-02 text-foreground-secondary mb-4">Frequently asked questions</p>
                            <Button asChild variant="outline" size="sm">
                                <Link to="/faq">Learn More</Link>
                            </Button>
                        </div>
                        <div className="bg-card rounded-lg p-6 text-center hover:shadow-lg transition-shadow">
                            <h3 className="text-body-01 font-semibold mb-2">E-Participation</h3>
                            <p className="text-body-02 text-foreground-secondary mb-4">Digital participation platform</p>
                            <Button asChild variant="outline" size="sm">
                                <Link to="/eparticipation">Participate</Link>
                            </Button>
                        </div>
                        <div className="bg-card rounded-lg p-6 text-center hover:shadow-lg transition-shadow">
                            <h3 className="text-body-01 font-semibold mb-2">Dashboard</h3>
                            <p className="text-body-02 text-foreground-secondary mb-4">Admin dashboard access</p>
                            <Button asChild variant="outline" size="sm">
                                <Link to="/dashboard">Enter</Link>
                            </Button>
                        </div>
                    </div>
                </Stack>
            </Stack>
        </div>
    )
}

export default index
